namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    using Data;
    using System.Xml.Serialization;
    using TeisterMask.DataProcessor.ImportDto;
    using System.IO;
    using System.Linq;
    using TeisterMask.Data.Models;
    using System.Text;
    using System.Globalization;
    using TeisterMask.Data.Models.Enums;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportProjectDto[]),
                new XmlRootAttribute("Projects"));

            var projectDtos = (ImportProjectDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var validProjects = new List<Project>();
            var result = new StringBuilder();

            foreach (var projectDto in projectDtos)
            {
                if (IsValid(projectDto) == false)
                {
                    result.AppendLine(ErrorMessage);
                }
                else
                {
                    var project = new Project()
                    {
                        Name = projectDto.Name,
                        OpenDate = DateTime.ParseExact(projectDto.OpenDate, @"dd/MM/yyyy", CultureInfo.InvariantCulture),
                        DueDate = string.IsNullOrEmpty(projectDto.DueDate) ?
                         (DateTime?)null : DateTime.ParseExact(projectDto.DueDate, @"dd/MM/yyyy", CultureInfo.InvariantCulture)
                    };

                    foreach (var taskDto in projectDto.Tasks)
                    {
                        bool isValidExecutionType = Enum.IsDefined(typeof(ExecutionType), taskDto.ExecutionType);
                        bool isValidLabelType = Enum.IsDefined(typeof(LabelType), taskDto.LabelType);

                        DateTime taskOpenDate = DateTime.ParseExact(taskDto.OpenDate, @"dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime projectOpenDate = project.OpenDate;

                        DateTime taskDueDate = DateTime.ParseExact(taskDto.DueDate, @"dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime? projectDueDate = project.DueDate;

                        if (IsValid(taskDto) == false
                            || isValidExecutionType == false
                            || isValidLabelType == false
                            || taskOpenDate < projectOpenDate
                            || taskDueDate > projectDueDate)
                        {
                            result.AppendLine(ErrorMessage);
                        }
                        else
                        {
                            var task = new Task()
                            {
                                Name = taskDto.Name,
                                OpenDate = taskOpenDate,
                                DueDate = taskDueDate,
                                ExecutionType = (ExecutionType)Enum.ToObject(typeof(ExecutionType), taskDto.ExecutionType),
                                LabelType = (LabelType)Enum.ToObject(typeof(LabelType), taskDto.LabelType)
                            };

                            project.Tasks.Add(task);
                        }
                    }

                    validProjects.Add(project);

                    result.AppendLine(string.Format(SuccessfullyImportedProject,
                                       project.Name,
                                       project.Tasks.Count));
                }
            }

            context.Projects.AddRange(validProjects);
            context.SaveChanges();

            return result.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var employeeDtos = JsonConvert.DeserializeObject<ImportEmployeeWithTaskDto[]>(jsonString);

            var validEmployees = new List<Employee>();
            var result = new StringBuilder();

            foreach (var employeeDto in employeeDtos)
            {
                if (IsValid(employeeDto) == false)
                {
                    result.AppendLine(ErrorMessage);
                }
                else
                {
                    var employee = new Employee()
                    {
                        Username = employeeDto.Username,
                        Email = employeeDto.Email,
                        Phone = employeeDto.Phone
                    };

                    foreach (var taskId in employeeDto.Tasks.Distinct())
                    {
                        var task = context.Tasks.Find(taskId);

                        if (task == null)
                        {
                            result.AppendLine(ErrorMessage);
                        }
                        else
                        {
                            employee.EmployeesTasks
                                .Add(new EmployeeTask
                                {
                                    TaskId = task.Id
                                });
                        }
                    }

                    validEmployees.Add(employee);

                    result.AppendLine(string.Format(SuccessfullyImportedEmployee,
                                       employee.Username,
                                       employee.EmployeesTasks.Count));
                }
            }

            context.Employees.AddRange(validEmployees);
            context.SaveChanges();

            return result.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}