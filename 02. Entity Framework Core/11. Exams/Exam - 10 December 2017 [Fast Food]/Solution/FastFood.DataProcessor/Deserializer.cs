using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using FastFood.Data;
using FastFood.DataProcessor.Dto.Import;
using FastFood.Models;
using FastFood.Models.Enums;
using Newtonsoft.Json;

namespace FastFood.DataProcessor
{
	public static class Deserializer
	{
		private const string ErrorMessage = "Invalid data format.";
		private const string SuccessMessage = "Record {0} successfully imported.";

		public static string ImportEmployees(FastFoodDbContext context, string jsonString)
		{
			var sb = new StringBuilder();

			var importEmployeeDtos = JsonConvert.DeserializeObject<List<ImportEmployeeDto>>(jsonString);
			var employeesToAdd = new List<Employee>();

			foreach (var importEmployeeDto in importEmployeeDtos)
			{
				if (!IsValid(importEmployeeDto))
                {
					sb.AppendLine(ErrorMessage);
					continue;
                }

				bool doesPositionExists = context.Positions.Any(x => x.Name == importEmployeeDto.Position);

				if (doesPositionExists == false)
                {
					var position = new Position()
					{
						Name = importEmployeeDto.Position
					};
					context.Positions.Add(position);
					context.SaveChanges();
                }

				var positionToAdd = context.Positions.SingleOrDefault(x => x.Name == importEmployeeDto.Position);

				var employee = new Employee()
				{
					Age = importEmployeeDto.Age,
					Name = importEmployeeDto.Name,
					Position = positionToAdd
				};

				employeesToAdd.Add(employee);
				sb.AppendLine(string.Format(SuccessMessage, employee.Name));
			}

			context.AddRange(employeesToAdd);
			context.SaveChanges();
			return sb.ToString().TrimEnd();
		}

		public static string ImportItems(FastFoodDbContext context, string jsonString)
		{
			var sb = new StringBuilder();

			var importItemDtos = JsonConvert.DeserializeObject<List<ImportItemDto>>(jsonString);
			var itemsToAdd = new List<Item>();

            foreach (var importItemDto in importItemDtos)
            {
				if (!IsValid(importItemDto))
                {
					sb.AppendLine(ErrorMessage);
					continue;
                }

				bool doesItemExists = context.Items.Any(x => x.Name == importItemDto.Name);
				if (doesItemExists)
				{
					sb.AppendLine(ErrorMessage);
					continue;
                }

				bool doesCategoryExists = context.Categories.Any(x => x.Name == importItemDto.Category); 
				if (!doesCategoryExists)
                {
					var category = new Category()
					{
						Name = importItemDto.Category
					};
					context.Categories.Add(category);
					context.SaveChanges();
                }

				var categoryToAdd = context.Categories.Where(x => x.Name == importItemDto.Category).FirstOrDefault();

				var item = new Item()
				{
					Name = importItemDto.Name,
					Price = importItemDto.Price,
					Category = categoryToAdd
				};

				context.Add(item);
				context.SaveChanges();
				sb.AppendLine(string.Format(SuccessMessage, item.Name));
			}

			return sb.ToString().TrimEnd();
		}

		public static string ImportOrders(FastFoodDbContext context, string xmlString)
		{

			var importOrderDtos = XMLConverter.Deserializer<ImportOrderDto>(xmlString, "Orders");
			var sb = new StringBuilder();
			var ordersToAdd = new List<Order>();

            foreach (var importOrderDto in importOrderDtos)
            {
				if(!IsValid(importOrderDto))
                {
					sb.AppendLine(ErrorMessage);
					continue;
                }

				bool doesOrderEmployeeExists = context
					.Employees
					.Any(x => x.Name == importOrderDto.Employee);
				if (!doesOrderEmployeeExists)
                {
					sb.AppendLine(ErrorMessage);
					continue;
				}

				DateTime time;
				var isTimeValid = DateTime.TryParseExact(
					importOrderDto.DateTime,
					"dd/MM/yyyy HH:mm", 
					CultureInfo.InvariantCulture, DateTimeStyles.None, 
					out time);

				if (!isTimeValid)
                {
					sb.AppendLine(ErrorMessage);
					continue;
				}

				var employee = context
					.Employees
					.SingleOrDefault(x => x.Name == importOrderDto.Employee);

				var order = new Order()
				{
					Customer = importOrderDto.Customer,
					DateTime = time,
					Employee = employee,
					Type = Enum.Parse<OrderType>(importOrderDto.Type)
				};
	
				bool doesExists = true;
                foreach (var orderItem in importOrderDto.Items)
                {
					var currentItemName = orderItem.Name;
					bool doesItemExists = context
						.Items
						.Any(x => x.Name == currentItemName);

					if (!doesItemExists)
                    {
						sb.AppendLine(ErrorMessage);
						doesExists = false;
						break;
					}

					var itemToAdd = context.Items.Where(x => x.Name == orderItem.Name).FirstOrDefault();

					int quantity = orderItem.Quantity;

                    var orderItemToAdd = new OrderItem()
                    {
						Item = itemToAdd,
						OrderId = order.Id,
						Quantity = quantity
					};
					order.OrderItems.Add(orderItemToAdd);
				}
				if (!doesExists)
                {
					sb.AppendLine(ErrorMessage);
					continue;
				}

				ordersToAdd.Add(order);
				sb.AppendLine($"Order for {order.Customer} on {order.DateTime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)} added");
			}
			context.Orders.AddRange(ordersToAdd);
			context.SaveChanges();
			return sb.ToString().TrimEnd();
		}

		private static bool IsValid(object obj)
		{
			var validationContext = new ValidationContext(obj);
			var validationResult = new List<ValidationResult>();

			bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
			return isValid;
		}
	}
}