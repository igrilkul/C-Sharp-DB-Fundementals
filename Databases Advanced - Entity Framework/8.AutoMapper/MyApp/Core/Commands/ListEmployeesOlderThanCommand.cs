using AutoMapper;
using MyApp.Core.Commands.Contracts;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyApp.Core.Commands
{
    public class ListEmployeesOlderThanCommand : ICommand
    {
        private readonly MyAppContext context;
        private readonly Mapper mapper;


        public ListEmployeesOlderThanCommand(MyAppContext context,Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] inputArgs)
        {
            int age = int.Parse(inputArgs[0]);
            var employees = context.Employees.Where(e => e.Birthday != null && DateTime.Now.Year - e.Birthday.Value.Year > age).OrderByDescending(t => t.Salary)
                .ToList();

            EmployeeInfoCommand employeeInfoCommand = new EmployeeInfoCommand(context, mapper);

            StringBuilder sb = new StringBuilder();
            foreach(var e in employees)
            {
                string[] id = new string[1];
                id[0] = e.Id.ToString();
                sb.AppendLine(employeeInfoCommand.Execute(id));
            }

            return sb.ToString().TrimEnd();
        }
    }
}
