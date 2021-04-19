 using System;
using System.IO;
using System.Linq;
using FastFood.Data;
using FastFood.Models.Enums;
using Newtonsoft.Json;

namespace FastFood.DataProcessor
{
	public class Serializer
	{
        public static string ExportOrdersByEmployee(FastFoodDbContext context, string employeeName, string orderType)
        {
            var employees = context
                .Employees
                .Where(e => e.Name == employeeName)
                .Select(e => new
                {
                    Name = e.Name,
                    Orders = e.Orders
                        .Where(o => o.Type.ToString() == orderType)
                        .Select(o => new
                        {
                            Customer = o.Customer,
                            Items = o.OrderItems
                                .Select(oi => new
                                {
                                    Name = oi.Item.Name,
                                    Price = decimal.Parse($"{oi.Item.Price:f2}"),
                                    Quantity = oi.Quantity
                                })
                                .ToList(),
                            TotalPrice = decimal.Parse($"{o.TotalPrice:f2}")
                        })
                        .OrderByDescending(o => o.TotalPrice)
                        .ThenByDescending(o => o.Items.Count)
                        .ToList(),
                    TotalMade = e.Orders
                        .Sum(o => decimal.Parse($"{o.TotalPrice:f2}"))
                })
                .ToList();

            var json = JsonConvert.SerializeObject(employees, Formatting.Indented);

            return json;
        }

            public static string ExportCategoryStatistics(FastFoodDbContext context, string categoriesString)
		{
			return null;
		}
	}
}