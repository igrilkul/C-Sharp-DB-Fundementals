using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Core;
using MyApp.Core.Contracts;
using MyApp.Data;
using System;
using System.Text;

namespace MyApp
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            PrintMenu();
            IServiceProvider services = ConfigureServices();
            IEngine engine = new Engine(services);
            engine.Run();
            //Db
            //Command pattern
            //DI
            //DTOs
            //Service layer
        }

        private static void PrintMenu()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Welcome to MyApp DB menu.");
            sb.AppendLine("List of commands:");
            sb.AppendLine(" - AddEmployee <FirstName> <LastName> <Salary>");
            sb.AppendLine(" - SetBirthday <EmployeeId> <Date(dd-MM-yyyy)>");
            sb.AppendLine(" - SetAddress <EmployeeId> <Address>");
            sb.AppendLine(" - EmployeeInfo <EmployeeId>");
            sb.AppendLine(" - EmployeePersonalInfo <EmployeeId>");
            sb.AppendLine(" - SetManager <EmployeeId> <ManagerId>");
            sb.AppendLine(" - ManagerInfo <ManagerId>");
            sb.AppendLine(" - ListEmployeesOlderThan <Age>");
            sb.AppendLine(" - Exit");
            Console.WriteLine(sb.ToString());
        }

        private static IServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDbContext<MyAppContext>(db => db.UseSqlServer(@"Server=.;Database=MyApp;Integrated Security=True;"));

            serviceCollection.AddTransient<ICommandInterpreter, CommandInterpreter>();
            serviceCollection.AddTransient<Mapper>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
