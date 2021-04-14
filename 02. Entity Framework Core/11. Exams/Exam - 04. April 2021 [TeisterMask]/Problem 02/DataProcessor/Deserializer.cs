namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;

    using Data;

    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    using Data;
    using System.Text;
    using TeisterMask;
    using TeisterMask.DataProcessor.ImportDto;
    using TeisterMask.Data.Models;
    using System.Globalization;
    using TeisterMask.Data.Models.Enums;
    using Newtonsoft.Json;
    using System.Linq;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var importProjectDtos = XMLConverter.Deserializer<ImportProjectDto>(xmlString, "Projects");
            var projectsToAdd = new List<Project>();


            foreach (var importProjectDto in importProjectDtos)
            {

                if (!IsValid(importProjectDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime projectOpenDate;
                bool isProjectOpenDateValid = DateTime.TryParseExact(importProjectDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out projectOpenDate);

                if (!isProjectOpenDateValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime? projectDueDate = null;

                if (!string.IsNullOrEmpty(importProjectDto.DueDate))
                {
                    DateTime tempProjectDueDate;
                    bool isValidProjectDueDate = DateTime.TryParseExact(importProjectDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempProjectDueDate);

                    if (!isValidProjectDueDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    else
                    {
                        projectDueDate = tempProjectDueDate;
                    }
                }


                var project = new Project()
                {
                    Name = importProjectDto.Name,
                    OpenDate = projectOpenDate,
                    DueDate = projectDueDate
                };

                foreach (var importTaskDto in importProjectDto.Tasks)
                {
                    if (!IsValid(importTaskDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var taskOpenDate = DateTime.ParseExact(importTaskDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var taskDueDate = DateTime.ParseExact(importTaskDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    if (taskOpenDate < projectOpenDate
                        || taskDueDate > projectDueDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var task = new Task()
                    {
                        Name = importTaskDto.Name,
                        OpenDate = taskOpenDate,
                        DueDate = taskDueDate,
                        ExecutionType = (ExecutionType)importTaskDto.ExecutionType,
                        LabelType = (LabelType)importTaskDto.LabelType
                    };

                    project.Tasks.Add(task);
                }
                projectsToAdd.Add(project);
                sb.AppendLine(string.Format(SuccessfullyImportedProject, project.Name, project.Tasks.Count));
            }
            context.AddRange(projectsToAdd);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var importEmployeeDtos = JsonConvert.DeserializeObject<List<ImportEmployeeDto>>(jsonString);
            var sb = new StringBuilder();
            var employeeToAdd = new List<Employee>();

            foreach (var importEmployeeDto in importEmployeeDtos)
            {
                if (!IsValid(importEmployeeDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var uniqueTasks = importEmployeeDto.Tasks.Distinct().ToList();

                var employee = new Employee()
                {
                    Username = importEmployeeDto.Username,
                    Email = importEmployeeDto.Email,
                    Phone = importEmployeeDto.Phone
                };

                foreach (var currentTask in uniqueTasks)
                {
                    bool doesTaskExist = context.Tasks.Any(x => x.Id == currentTask);

                    if (!IsValid(currentTask)
                        || !doesTaskExist)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var employeeTask = new EmployeeTask()
                    {
                        TaskId = currentTask,
                        Employee = employee
                    };

                    employee.EmployeesTasks.Add(employeeTask);
                }
                sb.AppendLine(string.Format(SuccessfullyImportedEmployee, employee.Username, employee.EmployeesTasks.Count));
                employeeToAdd.Add(employee);
            }
            context.Employees.AddRange(employeeToAdd);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}