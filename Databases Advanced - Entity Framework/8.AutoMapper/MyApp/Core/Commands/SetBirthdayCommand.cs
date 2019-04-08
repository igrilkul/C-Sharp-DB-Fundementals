using AutoMapper;
using MyApp.Core.Commands.Contracts;
using MyApp.Core.ViewModels;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyApp.Core.Commands
{
   public class SetBirthdayCommand:ICommand
    {
        private readonly MyAppContext context;

        public SetBirthdayCommand(MyAppContext context)
        {
            this.context = context;
        }

        public string Execute(string[] inputArgs)
        {
            int employeeId = int.Parse(inputArgs[0]);
            DateTime birthDate = DateTime.ParseExact(inputArgs[1],"dd-MM-yyyy",null);


            var employee = context.Employees.FirstOrDefault(e => e.Id == employeeId);
            
            if(employee==null)
            {
                throw new ArgumentNullException($"Emloyee with id {employeeId} not found.");
            }
            employee.Birthday = birthDate;

            this.context.SaveChanges();

            
            string result = $"Birthday {birthDate} added successfully to {employee.FirstName}.";

            return result;
        }
    }
}
