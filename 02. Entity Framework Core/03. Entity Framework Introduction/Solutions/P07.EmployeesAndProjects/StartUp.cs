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


            var result = GetEmployeesInPeriod(context);
            Console.WriteLine(result);
        }

        // 7. Employees and Projects

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var employees = context
                .Employees
                .Where(e => e.EmployeesProjects.Any(ep => ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003))
                .Take(10)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    ManagerFirstName = e.Manager.FirstName,
                    ManagerLastName = e.Manager.LastName,
                    Projects = e.EmployeesProjects
                               .Select(ep => new
                               {
                                   ProjectName = ep
                                                   .Project
                                                   .Name,
                                   StartDate = ep
                                                 .Project
                                                 .StartDate
                                                 .ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                                   EndDate = ep
                                               .Project
                                               .EndDate
                                               .HasValue ? ep
                                                             .Project
                                                             .EndDate
                                                             .Value
                                                             .ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                                                         : "not finished"
                               })
                })
                .ToList();

            foreach (var employee in employees)
            {
                stringBuilder.AppendLine($"{employee.FirstName} {employee.LastName} - Manager: {employee.ManagerFirstName} {employee.ManagerLastName}");

                foreach (var project in employee.Projects)
                {
                    stringBuilder.AppendLine($"--{project.ProjectName} - {project.StartDate} - {project.EndDate}");
                }
            }

            return stringBuilder.ToString().TrimEnd();
        }
    }
}