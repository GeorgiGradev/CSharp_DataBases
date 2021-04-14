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
            var result = GetEmployeesFullInformation(context);
            Console.WriteLine(result);
        }

        // 3. Employees Full Information 

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
           var employees = context
                .Employees
                .OrderBy(x=>x.EmployeeId)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
            }
                
            return sb.ToString().TrimEnd();
        }
    }
}
