using AutoMapper;
using MyApp.Core.Commands.Contracts;
using MyApp.Core.ViewModels;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Core.Commands
{
    public class EmployeePersonalInfoCommand : ICommand
    {
        private readonly MyAppContext context;
        private readonly Mapper mapper;

        public EmployeePersonalInfoCommand(MyAppContext context, Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] inputArgs)
        {
            int employeeId = int.Parse(inputArgs[0]);

            var employee = context.Employees.Find(employeeId);

            if (employee == null)
            {
                throw new ArgumentNullException("Employee not found.");
            }

            var employeeDto = this.mapper.CreateMappedObject<EmployeePersonalDto>(employee);

            string empty = "Not set.";
            string birthdate = string.IsNullOrEmpty(employeeDto.Birthday.ToString()) ? empty : employeeDto.Birthday.ToString();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"ID: {employeeId} - {employeeDto.FirstName} {employeeDto.LastName} - ${employeeDto.Salary:F2}");
            sb.AppendLine($"Birthday: {birthdate}");
            sb.AppendLine($"Address: {employeeDto.Address??empty}");
            return sb.ToString().TrimEnd();
        }
    }
}
