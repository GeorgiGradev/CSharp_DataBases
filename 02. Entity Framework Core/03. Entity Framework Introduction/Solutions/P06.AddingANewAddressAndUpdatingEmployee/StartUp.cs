using System;
using System.Linq;
using System.Text;
using SoftUni.Data;
using SoftUni.Models;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using var context = new SoftUniContext();


            var result = AddNewAddressToEmployee(context);
            Console.WriteLine(result);
        }

        // 6.Adding a New Address and Updating Employee

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            Address addressToAdd = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };
            Employee employeeNakov = context
                .Employees
                .First(e => e.LastName == "Nakov");
            employeeNakov.Address = addressToAdd;

            context.SaveChanges();

            var addresses = context
                .Employees
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .Select(e => new
                {
                    e.Address.AddressText
                })
                .ToList();

            foreach (var a in addresses)
            {
                sb.AppendLine(a.AddressText);
            }

            return sb.ToString().TrimEnd();
        }
    }
}
