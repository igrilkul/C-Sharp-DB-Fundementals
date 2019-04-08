using BillsPaymentSystem.App.Core.Contracts;
using BillsPaymentSystem.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillsPaymentSystem.App.Core
{
    public class Engine : IEngine
    {
        private readonly ICommandInterpreter commandInterpreter;

        public Engine(ICommandInterpreter commandInterpreter)
        {
            this.commandInterpreter = commandInterpreter;
        }
        public void Run()
        {
            
            PrintMenu();
            while(true)
            {
                string[] inputParams = Console.ReadLine()
               .Split(" ", StringSplitOptions.RemoveEmptyEntries);

                using (BillsPaymentSystemContext context = new BillsPaymentSystemContext())
                {
                    try
                    {
                        string result = this.commandInterpreter.Read(inputParams, context);
                        Console.WriteLine(result);
                    }
                    catch(ArgumentException an)
                    {
                        Console.WriteLine(an.Message);
                    }
                }
            }
           

        }

        private void PrintMenu()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Welcome to BillsPaymentSystem.App");
            sb.AppendLine("List of commands:");
            sb.AppendLine("  --UserInfo [userId] - Prints user's cc and bank accounts' info.");
            sb.AppendLine("  --Deposit [bankAccountId] [amount] - Deposits amount to desired bank account.");
            sb.AppendLine("  --Withdraw [userId] [amount] - Withdraws amount from user's bank accounts and credit cards.");
            Console.WriteLine(sb.ToString());
        }
    }
}
