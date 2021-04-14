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

            var result = DeleteProjectById(context);
            Console.WriteLine(result);
        }

        //14. Delete Project by Id

        public static string DeleteProjectById(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var contextToRemove = context
                .EmployeesProjects
                .Where(ep => ep.ProjectId == 2);

            foreach (var c in contextToRemove)
            {
                context.Remove(c);
            }

            var contextToRemove2 = context
                .Projects
                .Where(p => p.ProjectId == 2);

            foreach (var c in contextToRemove2)
            {
                context.Remove(c);
            }

            context.SaveChanges();


            var projects = context
                .Projects
                .Take(10)
                .Select(p => new
                {
                    p.Name
                });

            foreach (var p in projects)
            {
                sb.AppendLine($"{p.Name}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
