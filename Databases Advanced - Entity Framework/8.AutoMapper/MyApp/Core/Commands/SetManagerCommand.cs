using AutoMapper;
using MyApp.Core.Commands.Contracts;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyApp.Core.Commands
{
   public class SetManagerCommand : ICommand
    {
        private readonly MyAppContext context;
        

        public SetManagerCommand(MyAppContext context)
        {
            this.context = context;
        }

        public string Execute(string[] inputArgs)
        {
            int employeeId = int.Parse(inputArgs[0]);
            int managerId = int.Parse(inputArgs[1]);

            var employee = context.Employees.Find(employeeId);
            var manager = context.Employees.Find(managerId);

            if(employee==null)
            {
                throw new ArgumentNullException($"Employee {employeeId} not found.");
            }
            if(manager==null)
            {
                throw new ArgumentNullException($"Manager {managerId} not found.");
            }

            employee.Manager = manager;
            this.context.SaveChanges();
            return "Manager set successfully.";
        }
    }
}
