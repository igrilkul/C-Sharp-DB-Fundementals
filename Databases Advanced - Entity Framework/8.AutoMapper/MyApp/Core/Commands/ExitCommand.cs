using MyApp.Core.Commands.Contracts;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Core.Commands
{
    public class ExitCommand : ICommand
    {
        private readonly MyAppContext context;
       

        public ExitCommand(MyAppContext context)
        {
            this.context = context;
        }

        public string Execute(string[] inputArgs)
        {
            this.context.Dispose();
            string exit = "Application closed successfully.";
            Environment.Exit(0);
            return exit;
        }
    }
}
