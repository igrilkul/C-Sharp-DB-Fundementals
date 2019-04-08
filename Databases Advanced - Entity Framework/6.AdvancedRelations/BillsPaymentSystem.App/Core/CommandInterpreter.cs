using BillsPaymentSystem.App.Core.Commands.Contracts;
using BillsPaymentSystem.App.Core.Contracts;
using BillsPaymentSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BillsPaymentSystem.App.Core
{
    public class CommandInterpreter : ICommandInterpreter
    {
        private const string Suffix = "Command";

        public string Read(string[] inputParams,BillsPaymentSystemContext context)
        {
            string command = inputParams[0];
            string[] args = inputParams.Skip(1).ToArray();

            var type = Assembly.GetCallingAssembly()
                .GetTypes().FirstOrDefault(x => x.Name == command + Suffix);

            var typeInstance = Activator.CreateInstance(type, context);

            var result = ((ICommand)typeInstance).Execute(args);

            return result;
        }
    }
}
