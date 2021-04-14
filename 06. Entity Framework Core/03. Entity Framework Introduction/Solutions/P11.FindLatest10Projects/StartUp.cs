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

            var result = GetLatestProjects(context);
            Console.WriteLine(result);
        }

        // 11.Find Latest 10 Projects

        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var startedProjects = context
                .Projects
                .OrderByDescending(x => x.StartDate)
                .Take(10)
                .Select(x=> new
                {
                    x.Name,
                    x.Description,
                    startDate = x.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                })
                .OrderBy(x=>x.Name)
                .ToList();

            foreach (var sp in startedProjects)
            {
                sb.AppendLine($"{sp.Name}");
                sb.AppendLine($"{sp.Description}");
                sb.AppendLine($"{sp.startDate}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
