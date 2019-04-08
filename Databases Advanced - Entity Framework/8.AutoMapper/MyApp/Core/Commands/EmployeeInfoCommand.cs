using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Core.Commands.Contracts;
using MyApp.Core.ViewModels;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyApp.Core.Commands
{
   public class EmployeeInfoCommand:ICommand
    {
        private readonly MyAppContext context;
        private readonly Mapper mapper;

        public EmployeeInfoCommand(MyAppContext context,Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] inputArgs)
        {
            int employeeId = int.Parse(inputArgs[0]);

            var employee = context.Employees
                .Include(m => m.Manager)
                .FirstOrDefault(e => e.Id == employeeId);

            if (employee == null)
            {
                throw new ArgumentNullException("Employee not found.");
            }

            var employeeDto = this.mapper.CreateMappedObject<EmployeeDto>(employee);
            string managerName = employeeDto.Manager == null ? "[No manager]" : employeeDto.Manager.FirstName;
            string result = $"ID: {employeeId} - {employeeDto.FirstName} {employeeDto.LastName} - ${employeeDto.Salary:F2} - {managerName}";
            return result;
        }
    }
}
