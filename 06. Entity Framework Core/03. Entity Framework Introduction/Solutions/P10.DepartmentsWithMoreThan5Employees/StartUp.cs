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

            var result = GetDepartmentsWithMoreThan5Employees(context);
            Console.WriteLine(result);
        }

        // 10. Departments with More Than 5 Employees

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var departments = context
                .Departments
                .Where(d => d.Employees.Count() > 5)
                .OrderBy(d=>d.Employees.Count())
                .ThenBy(d=>d.Name)
                .Select(d => new
                {
                    departmenName = d.Name,
                    managerFirstName = d.Manager.FirstName,
                    managerLastName = d.Manager.LastName,
                    depEmployees = d.Employees
                           .Select(e => new
                           {
                               employeeFirstName = e.FirstName,
                               employeeLastName = e.LastName,
                               employeeJobTitle = e.JobTitle
                           })
                           .OrderBy(e => e.employeeFirstName)
                           .ThenBy(e => e.employeeLastName)
                           .ToList()
                })

                .ToList();


            foreach (var dep in departments)
            {
                sb.AppendLine($"{dep.departmenName} - {dep.managerFirstName} {dep.managerLastName}");
                foreach (var emp in dep.depEmployees)
                {
                    sb.AppendLine($"{emp.employeeFirstName} {emp.employeeLastName} - {emp.employeeJobTitle}");   
                }
            }


            return sb.ToString().TrimEnd();
        }
    }
}
