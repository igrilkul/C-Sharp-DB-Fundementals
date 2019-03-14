using System;

namespace MiniORM.App
{
    using Data;
    using Data.Entities;
    using System.Linq;

    public class Program
    {
      public static void Main(string[] args)
        {
            var connectionString = "Server=.;Database=MiniORM;Integrated Security=True";

            var context = new SoftUniDbContextClass(connectionString);

            context.Employees.Add(new Employees
            {
                FirstName = "Gosho",
                LastName = "Inserted",
                DepartmentId = context.Departments.First().Id,
                IsEmployed = true
            });

            var employee = context.Employees.Last();
            employee.FirstName = "Modified";

            context.SaveChanges();
        }
    }
}
