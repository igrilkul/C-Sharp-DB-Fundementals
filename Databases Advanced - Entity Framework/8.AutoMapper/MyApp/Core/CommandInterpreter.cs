using Microsoft.Extensions.DependencyInjection;
using MyApp.Core.Commands.Contracts;
using MyApp.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MyApp.Core
{
    public class CommandInterpreter : ICommandInterpreter
    {
        private const string Suffix = "Command";
        private readonly IServiceProvider provider;

        public CommandInterpreter(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public string Read(string[] inputArgs)
        {
            string commandName = inputArgs[0]+Suffix;
            string[] commandParams = inputArgs.Skip(1).ToArray();

            var type = Assembly.GetCallingAssembly()
                .GetTypes()
                .FirstOrDefault(x => x.Name == commandName);

            if(type == null)
            {
                throw new ArgumentNullException("Invalid command!");
            }

            var constructor = type.GetConstructors()
                .FirstOrDefault();

            var constructorParams = constructor.GetParameters()
                .Select(x => x.ParameterType)
                .ToArray();

            var services = constructorParams
                .Select(this.provider.GetService)
                .ToArray();

            //unsure
            var command = (ICommand)constructor.Invoke(services);
            string result = command.Execute(commandParams);

            return result;
            //get type

            //get ctor
            //ctor params
            //invoke ctor
            //execute
            //return result
        }
    }
}
