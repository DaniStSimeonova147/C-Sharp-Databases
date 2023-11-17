using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (SoftUniContext context = new SoftUniContext())
            {
                Console.WriteLine(RemoveTown(context));
            }
        }

        //Problem03
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                           .Select(e => new
                           {
                               EmployeeId = e.EmployeeId,
                               FirstName = e.FirstName,
                               MiddleName = e.MiddleName,
                               LastName = e.LastName,
                               JobTitle = e.JobTitle,
                               Salary = e.Salary
                           })
                           .OrderBy(e => e.EmployeeId)
                           .ToList();

            StringBuilder resulrt = new StringBuilder();

            foreach (var employee in employees)
            {
                resulrt.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} " +
                                   $"{employee.JobTitle} {employee.Salary:F2}");

            }

            return resulrt.ToString().TrimEnd();
        }


        //Problem04
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                           .Select(e => new
                           {
                               FirstName = e.FirstName,
                               Salary = e.Salary
                           })
                           .Where(e => e.Salary > 50000)
                           .OrderBy(e => e.FirstName)
                           .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var employee in employees)
            {
                result.AppendLine($"{employee.FirstName} - {employee.Salary:f2}");
            }

            return result.ToString().TrimEnd();
        }
        //Problem05
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    DepartmantName = e.Department.Name,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Salary = e.Salary
                })
                .Where(e => e.DepartmantName == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var employee in employees)
            {
                result.AppendLine($"{employee.FirstName} {employee.LastName} from " +
                    $"{employee.DepartmantName} - ${employee.Salary:f2}");
            }

            return result.ToString().TrimEnd();
        }

        //Problem06
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address address = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            Employee employee = context.Employees.FirstOrDefault(e => e.LastName == "Nakov");

            employee.Address = address;

            context.SaveChanges();

            var employees = context.Employees
                            .OrderByDescending(e => e.AddressId)
                            .Select(a => a.Address.AddressText)
                            .Take(10)
                            .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var emp in employees)
            {
                result.AppendLine(emp);
            }

            return result.ToString().TrimEnd();
        }

        //Problem07
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            List<Employee> employees = context.Employees
                .Include(e => e.Manager)
                .Include(e => e.EmployeesProjects)
                .ThenInclude(e => e.Project)
                .Where(employee => employee.EmployeesProjects
                .Any(project => project.Project.StartDate.Year >= 2001
                              && project.Project.StartDate.Year <= 2003))
                .Take(10)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var employee in employees)
            {
                result.AppendLine($"{employee.FirstName} {employee.LastName} - Manager: {employee.Manager.FirstName} {employee.Manager.LastName}");

                foreach (var project in employee.EmployeesProjects)
                {
                    result.AppendLine($"--{project.Project.Name} - " +
                        $"{project.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)} - " +
                        $"{(project.Project.EndDate == null ? "not finished" : project.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture))}");
                }
            }

            return result.ToString().TrimEnd();
        }

        //Problem08
        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses
                .Select(a => new
                {
                    AddressText = a.AddressText,
                    TownName = a.Town.Name,
                    Employees = a.Employees.Count()
                })
                .OrderByDescending(a => a.Employees)
                .ThenBy(a => a.TownName)
                .ThenBy(a => a.AddressText)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var address in addresses)
            {
                result.AppendLine($"{address.AddressText}, {address.TownName} - {address.Employees} employees");
            }

            return result.ToString().TrimEnd();

        }

        //Problem09
        public static string GetEmployee147(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    JobTitle = e.JobTitle,
                    PrjNames = e.EmployeesProjects
                    .Select(p => new
                    {
                        Name = p.Project.Name
                    })
                    .OrderBy(p => p.Name)
                    .ToList()
                })
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var employee in employees)
            {
                result.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

                foreach (var project in employee.PrjNames)
                {
                    result.AppendLine(project.Name);
                }
            }

            return result.ToString().TrimEnd();
        }

        //Problem10
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Where(d => d.Employees.Count() > 5)
                .OrderBy(d => d.Employees.Count())
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    DepName = d.Name,
                    ManagerFirstname = d.Manager.FirstName,
                    ManagerLastName = d.Manager.LastName,
                    Employees = d.Employees
                    .Select(e => new
                    {
                        EmployeeFirstName = e.FirstName,
                        EmployeeLastName = e.LastName,
                        JobTitle = e.JobTitle
                    })
                    .OrderBy(e => e.EmployeeFirstName)
                    .ThenBy(e => e.EmployeeLastName)
                    .ToList()
                })
                .ToList();


            StringBuilder resut = new StringBuilder();

            foreach (var department in departments)
            {
                resut.AppendLine($"{department.DepName} - {department.ManagerFirstname} {department.ManagerLastName}");

                foreach (var employee in department.Employees)
                {
                    resut.AppendLine($"{employee.EmployeeFirstName} {employee.EmployeeLastName} - {employee.JobTitle}");

                }
            }

            return resut.ToString().TrimEnd();
        }

        //Problem11
        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .Select(p => new
                {
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate
                })
                .OrderBy(d => d.Name)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var project in projects)
            {
                result.AppendLine($"{project.Name}")
                       .AppendLine($"{project.Description}")
                       .AppendLine($"{project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}");
            }

            return result.ToString().TrimEnd();
        }

        //Problem12
        public static string IncreaseSalaries(SoftUniContext context)
        {
            List<Employee> employees = context.Employees
                 .Where(e => e.Department.Name == "Engineering" ||
                  e.Department.Name == "Tool Design" ||
                  e.Department.Name == "Marketing" ||
                  e.Department.Name == "Information Services")
                 .ToList();

            List<Employee> employeesWithUpdateSalary = new List<Employee>();

            foreach (var employee in employees)
            {
                employee.Salary *= 1.12m;
                context.SaveChanges();
                employeesWithUpdateSalary.Add(employee);

            }

            StringBuilder result = new StringBuilder();

            foreach (var emp in employeesWithUpdateSalary.OrderBy(e => e.FirstName).ThenBy(e => e.LastName))
            {
                result.AppendLine($"{emp.FirstName} {emp.LastName} (${emp.Salary:f2})");
            }

            return result.ToString().TrimEnd();
        }

        //Problem13
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.FirstName.StartsWith("Sa"))
                .Select(e => new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    JobTitle = e.JobTitle,
                    Salary = e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var employee in employees)
            {
                result.AppendLine($"{employee.FirstName} {employee.LastName} - " +
                    $"{employee.JobTitle} - (${employee.Salary:F2})");
            }

            return result.ToString().TrimEnd();
        }

        //Problem14
        public static string DeleteProjectById(SoftUniContext context)
        {
            Project projectId = context.Projects.FirstOrDefault(p => p.ProjectId == 2);
            EmployeeProject empProjectId = context.EmployeesProjects.FirstOrDefault(ep => ep.ProjectId == 2);

            context.EmployeesProjects.Remove(empProjectId);
            context.Projects.Remove(projectId);

            context.SaveChanges();

            var projects = context.Projects
                .Select(p => new
                {
                    Name = p.Name
                })
                .Take(10)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var project in projects)
            {
                result.AppendLine(project.Name);
            }

            return result.ToString().TrimEnd();
        }

        //Problem15
        public static string RemoveTown(SoftUniContext context)
        {
            List<Employee> employees = context.Employees
                 .Where(e => e.Address.Town.Name == "Seattle")
                 .ToList();

            foreach (var employee in employees)
            {
                employee.AddressId = null;
                context.SaveChanges();
            }

            List<Address> addresses = context.Addresses
                .Where(a => a.Town.Name == "Seattle")
                .ToList();

            int count = addresses.Count();

            foreach (var address in addresses)
            {
                context.Remove(address);
                context.SaveChanges();
            }

            List<Town> towns = context.Towns
                  .Where(t => t.Name == "Seattle")
                  .ToList();

            foreach (var town in towns)
            {
                context.Remove(town);
                context.SaveChanges();
            }

            return $"{count} addresses in Seattle were deleted";
        }
    }
}
