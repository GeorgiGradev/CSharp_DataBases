using System;
using System.Linq;
using System.Text;
using SoftUni.Data;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using var context = new SoftUniContext();
            var result = GetEmployeesFromResearchAndDevelopment(context);
            Console.WriteLine(result);
        }

        // 5. Employees from Research and Development

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context
                .Employees
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName, 
                    DepartmentName = e.Department.Name,
                    e.Salary
                })
                .OrderBy(e=> e.Salary)
                .ThenByDescending(e=>e.FirstName)
                .ToList();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} from {e.DepartmentName} - ${e.Salary:f2}");
            }


            return sb.ToString().TrimEnd();
        }
    }
}
