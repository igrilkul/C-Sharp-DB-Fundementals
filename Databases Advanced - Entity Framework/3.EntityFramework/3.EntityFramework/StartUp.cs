using System;
using SoftUni.Models;
using SoftUni.Data;
using System.Linq;
using System.Text;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace SoftUni
{
   public class StartUp
    {
        static void Main(string[] args)
        {
            
            using (SoftUniContext context = new SoftUniContext())
            {
                Console.WriteLine(RemoveTown(context));
            }
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary,
                    e.EmployeeId
                })
                .OrderBy(x => x.EmployeeId);
            StringBuilder sb = new StringBuilder();

            foreach(var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
               .Where(e => e.Salary > 50000)
               .Select(e => new
               {
                   e.FirstName,
                   e.Salary
               })
               .OrderBy(x => x.FirstName);

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} - {e.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext
context)
        {
            var employees = context.Employees
                .Where(d => d.Department.Name == "Research and Development")
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.Department.Name,
                    x.Salary
                })
                .OrderBy(x => x.Salary).ThenByDescending(x=>x.FirstName);

            StringBuilder sb = new StringBuilder();
            foreach(var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} from {e.Name} - ${e.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
            
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var address = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            var nakov = context.Employees.FirstOrDefault(e => e.LastName == "Nakov");

            nakov.Address = address;

            context.SaveChanges();

            var employeeAddresses = context.Employees
                .OrderByDescending(x => x.AddressId)
                .Select(a => a.Address.AddressText)
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();
            foreach(var ea in employeeAddresses)
            {
                sb.AppendLine($"{ea}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(p => p.EmployeesProjects.Any(e => e.Project.StartDate.Year >= 2001 && e.Project.StartDate.Year <= 2003))
                .Select(x => new
                {
                    EmployeeFullName = x.FirstName + " " + x.LastName,
                    ManagerFullName = x.Manager.FirstName + " " + x.Manager.LastName,
                    Projects = x.EmployeesProjects.Select(p => new
                    {
                        ProjectName = p.Project.Name,
                        StartDate = p.Project.StartDate,
                        EndDate = p.Project.EndDate
                    }).ToList()
                })
                .Take(10).ToList()
                ;
            StringBuilder sb = new StringBuilder();

            foreach(var e in employees)
            {
                sb.AppendLine($"{e.EmployeeFullName} - Manager: {e.ManagerFullName}");
                foreach(var p in e.Projects)
                {
                    var startDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    var endDate = p.EndDate.HasValue ?
                        p.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt",CultureInfo.InvariantCulture)
                        : "not finished";

                    sb.AppendLine($"--{p.ProjectName} - {startDate} - {endDate}");
                }
            }

            return sb.ToString();
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses
                .OrderByDescending(p => p.Employees.Count)
                .ThenBy(p => p.Town.Name)
                .ThenBy(p => p.AddressText)
                .Select(x => new
                {
                    x.AddressText,
                    x.Town.Name,
                    x.Employees.Count
                })
                .Take(10).ToList();

            StringBuilder sb = new StringBuilder();
            foreach(var a in addresses)
            {
                sb.AppendLine($"{a.AddressText}, {a.Name} - {a.Count} employees");
            }
            return sb.ToString().TrimEnd()
;
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            int employeeId = 147;
            var employee = context.Employees
                .Where(e => e.EmployeeId == employeeId)
                .Select(x => new
                {
                    employeeName = x.FirstName + " " + x.LastName,
                    x.JobTitle,
                    Projects = x.EmployeesProjects.Select(p => new
                    {
                        ProjectName = p.Project.Name
                    }).OrderBy(p => p.ProjectName)
.ToList()

                }).ToList();

            StringBuilder sb = new StringBuilder();
            foreach(var e in employee)
            {
                sb.AppendLine($"{e.employeeName} - {e.JobTitle}");
                foreach(var p in e.Projects)
                {
                    sb.AppendLine($"{p.ProjectName}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext
context)
        {
            var departments = context.Departments
                .Where(d => d.Employees.Count > 5)
                .OrderBy(d => d.Employees.Count > 5)
                .ThenBy(e => e.Name)
                .Select(x => new
                {
                    DepartmentName = x.Name,
                    ManagerName = x.Manager.FirstName + " " + x.Manager.LastName,
                    Employees = x.Employees.Select(t => new
                    {
                        t.FirstName,
                        t.LastName,
                        t.JobTitle
                    }).OrderBy(y => y.FirstName).ThenBy(s => s.LastName).ToList()
                }).ToList();

            StringBuilder sb = new StringBuilder();
            foreach(var d in departments)
            {
                sb.AppendLine($"{d.DepartmentName} - {d.ManagerName}");
                foreach(var e in d.Employees)
                {
                    sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                }
            }

            return sb.ToString();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                .Select(x => new
                {
                    x.Name,
                    x.Description,
                    x.StartDate
                })
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .OrderBy(t=>t.Name)
                .ToList();

            StringBuilder sb = new StringBuilder();
            foreach(var p in projects)
            {
                var startDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

                sb.AppendLine($"{p.Name}");
                sb.AppendLine($"{p.Description}");
                sb.AppendLine($"{startDate}");
            }

            return sb.ToString();
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            var employeesToIncrease = context.Employees
                .Where(e => e.Department.Name == "Engineering" || e.Department.Name == "Tool Design" || e.Department.Name == "Marketing" || e.Department.Name == "Information Services")
                //turns to read only when select is used
                //.Select(x => new
                //{
                //    x.FirstName,
                //    x.LastName,
                //    x.Salary,
                //})
                .OrderBy(t => t.FirstName).ThenBy(y => y.LastName)
                .ToList();

            var sb = new StringBuilder();
            foreach (var e in employeesToIncrease)
            {
                e.Salary *= (decimal)1.12;
                sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:F2})");
            }

            context.SaveChanges();
            return sb.ToString().TrimEnd();

        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext
context)
        {
            var employees = context.Employees
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.JobTitle,
                    x.Salary
                }).Where(e => EF.Functions.Like(e.FirstName,"Sa%"))
                .OrderBy(y => y.FirstName)
                .ThenBy(s => s.LastName)
                .ToList();

            StringBuilder sb = new StringBuilder();
            foreach(var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:F2})");
            }

            return sb.ToString().TrimEnd();
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            var project = context.Projects.Find(2);

            var toRemove = context.EmployeesProjects.Where(t => t.ProjectId == 2).ToList();
            context.EmployeesProjects.RemoveRange(toRemove);

            context.Projects.Remove(project);

            context.SaveChanges();

            var projects = context.Projects
                .Select(x=>new
                {
                x.Name
                 })
            .Take(10)
            .ToList();

            StringBuilder sb = new StringBuilder();
            foreach(var p in projects)
            {
                sb.AppendLine(p.Name);
            }

            return sb.ToString().TrimEnd();
        }

        public static string RemoveTown(SoftUniContext context)
        {
            var townToRemove = context.Towns.FirstOrDefault(t => t.Name == "Seattle");
            var townToRemoveId = townToRemove.TownId;

            var employees = context.Employees
                .Where(e => e.Address.TownId == townToRemoveId).ToList();

            foreach(var e in employees)
            {
                e.AddressId = null;
            }

            var addressses = context.Addresses.Where(x => x.TownId == townToRemoveId).ToList();

            int count = addressses.Count;

            context.Addresses.RemoveRange(addressses);
            context.Towns.Remove(townToRemove);

            StringBuilder sb = new StringBuilder();
            sb.Append($"{count} addresses in Seattle were deleted");
            return sb.ToString();
        }
    }
}
//"Server=DESKTOP-A97NGR1\\SQLEXPRESS;Database=SoftUni;Integrated Security=True"

    //Scaffold-DbContext -Connection "Server=DESKTOP-A97NGR1\SQLEXPRESS;Database=SoftUni;Integrated Security=True;" -Provider Microsoft.EntityFrameworkCore.SqlServer -OutputDir Data/Models