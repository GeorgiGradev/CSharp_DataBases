namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var result = context
                .Projects
                .ToList()
                .Where(project => project.Tasks.Any())
                .Select(project => new ExportProjectDto
                {
                    TasksCount = project.Tasks.Count,
                    Name = project.Name,
                    HasEndDate = project.DueDate.HasValue ? "Yes" : "No",
                    Tasks = project.Tasks.Select(task => new ExportTaksDto
                    {
                        Name = task.Name,
                        LabelType = task.LabelType.ToString()
                    })
                    .OrderBy(task => task.Name)
                    .ToList()
                })
                .OrderByDescending(project => project.TasksCount)
                .ThenBy(project => project.Name)
                .ToList();

            var xml = XMLConverter.Serialize(result, "Projects");

            return xml;


            //var projects = context.Projects
            //    .Where(p => p.Tasks.Count > 0)
            //   .ToArray() 
            //    .Select(p => new ExportProjectDto()
            //    {
            //        Name = p.Name,
            //        TasksCount = p.Tasks.Count,
            //        HasEndDate = p.DueDate.HasValue ? "Yes" : "No",
            //        Tasks = p.Tasks
            //                    .Select(t => new ExportTaksDto()
            //                    {
            //                        Name = t.Name,
            //                        LabelType = t.LabelType.ToString()
            //                    })
            //                    .OrderBy(t => t.Name)
            //                    .ToArray()
            //    })
            //    .OrderByDescending(p => p.TasksCount)
            //    .ThenBy(p => p.Name)
            //    .ToArray();

            //var result = new StringBuilder();

            //var xmlSerializer = new XmlSerializer(typeof(ExportProjectDto[]), new XmlRootAttribute("Projects"));
            //var xmlNamespaces = new XmlSerializerNamespaces();
            //xmlNamespaces.Add(string.Empty, string.Empty);

            //using (var writter = new StringWriter(result))
            //{
            //    xmlSerializer.Serialize(writter, projects, xmlNamespaces);
            //}

            //return result.ToString().TrimEnd();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var result = context
            .Employees
            .Where(emp => emp.EmployeesTasks.Any(empTask => empTask.Task.OpenDate >= date))
            .Select(emp => new
            {
                Username = emp.Username,
                Tasks = emp.EmployeesTasks
                    .Where(empTask => empTask.Task.OpenDate > date)
                    .OrderByDescending(emp => emp.Task.DueDate)
                    .ThenBy(emp => emp.Task.Name)
                    .Select(empTask => new
                    {
                        TaskName = empTask.Task.Name,
                        OpenDate = empTask.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                        DueDate = empTask.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                        LabelType = empTask.Task.LabelType.ToString(),
                        ExecutionType = empTask.Task.ExecutionType.ToString(),
                    })
                .ToList()
            })
            .ToList()
            .OrderByDescending(emp => emp.Tasks.Count())
            .ThenBy(emp => emp.Username)
            .Take(10)
            .ToList();


            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            return json;
        }
    }
}