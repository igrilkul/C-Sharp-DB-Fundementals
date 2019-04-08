using MyApp.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace MyApp.Core
{
   public class Engine:IEngine
    {
        private readonly IServiceProvider provider;

        public Engine(IServiceProvider provider)
        {
            this.provider = provider;
        }

       public void Run()
        {
            while (true)
            {

                string[] inputArgs = Console.ReadLine().Split(" ", StringSplitOptions.RemoveEmptyEntries).ToArray();

                //using dependancyInjection
                var commandInterpreter = this.provider.GetService<ICommandInterpreter>();
                string result = commandInterpreter.Read(inputArgs);

                Console.WriteLine(result);

                //TODO add try catch block
            }
        }

    }
}
