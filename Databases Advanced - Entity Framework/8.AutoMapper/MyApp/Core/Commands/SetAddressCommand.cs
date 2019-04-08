using MyApp.Core.Commands.Contracts;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyApp.Core.Commands
{
    public class SetAddressCommand : ICommand
    {
        private readonly MyAppContext context;

        public SetAddressCommand(MyAppContext context)
        {
            this.context = context;
        }

        public string Execute(string[] inputArgs)
        {
            int employeeId = int.Parse(inputArgs[0]);
            string address = string.Join(" ",inputArgs.Skip(1));

            var employee = context.Employees.Find(employeeId);
            if(employee == null)
            {
                throw new ArgumentNullException("Employee not found.");
            }

            employee.Address = address;
            context.SaveChanges();

            return "Address added successfully.";
        }
    }
}
