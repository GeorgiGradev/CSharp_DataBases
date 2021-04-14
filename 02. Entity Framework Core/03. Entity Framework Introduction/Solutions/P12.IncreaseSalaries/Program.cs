using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using var context = new SoftUniContext();

            var result = IncreaseSalaries(context);
            Console.WriteLine(result);
        }

        // 12. Increase Salaries

        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var salaryIncreasement = 1.12M;

            var targetingDepartments = new string[]
            {
                "Engineering",
                "Tool Design",
                "Marketing",
                "Information Services"
            };

            var targetingEmployees = context.Employees
                .Where(e => targetingDepartments.Contains(e.Department.Name))
                .ToList();

            foreach (var employee in targetingEmployees)
            {
                employee.Salary *= salaryIncreasement;
            }

            context.SaveChanges();

            var employees = targetingEmployees
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            foreach (var employee in employees)
            {
                stringBuilder.AppendLine($"{employee.FirstName} {employee.LastName} (${employee.Salary:f2})");
            }

            return stringBuilder.ToString().TrimEnd();
        }
    }
}
