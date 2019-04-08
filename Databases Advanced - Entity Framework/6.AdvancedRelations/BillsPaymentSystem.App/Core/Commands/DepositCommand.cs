using BillsPaymentSystem.App.Core.Commands.Contracts;
using BillsPaymentSystem.Data;
using BillsPaymentSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BillsPaymentSystem.App.Core.Commands
{
    public class DepositCommand : ICommand
    {
        private readonly BillsPaymentSystemContext context;

        public DepositCommand(BillsPaymentSystemContext context)
        {
            this.context = context;
        }

        public string Execute(string[] args)
        {
            int bankId = int.Parse(args[0]);
            decimal amount = decimal.Parse(args[1]);

            string result = Deposit(bankId, amount);
            return result;
        }

        private string Deposit(int bankId, decimal amount)
        {
            var account = this.context.BankAccounts.FirstOrDefault(x => x.BankAccountId == bankId);

            if(account == null)
            {
                throw new ArgumentException("Bank account not found.");
            }
            else
            {
                account.Balance += amount;
                context.SaveChanges();
            }
            string result = $"${amount} Deposited successfully.";
            return result;
        }
    }
}
