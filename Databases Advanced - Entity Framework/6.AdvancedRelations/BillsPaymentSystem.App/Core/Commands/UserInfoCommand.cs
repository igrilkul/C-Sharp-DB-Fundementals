using BillsPaymentSystem.App.Core.Commands.Contracts;
using BillsPaymentSystem.Data;
using BillsPaymentSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BillsPaymentSystem.App.Core.Commands
{
    public class UserInfoCommand : ICommand
    {
        private readonly BillsPaymentSystemContext context;

        public UserInfoCommand(BillsPaymentSystemContext context)
        {
            this.context = context;
        }

        public string Execute(string[] args)
        {
            int userId = int.Parse(args[0]);

            var user = this.context.Users.FirstOrDefault(u => u.UserId == userId);

            if(user == null)
            {
                throw new ArgumentException($"User with id {userId} not found!");
            }
            else
            {
               string result = PrintUserDetails(user);
               return result;
            }
        }

        private string PrintUserDetails(User user)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"User: {user.FirstName} {user.LastName}");
            sb.AppendLine("Bank Accounts:");

            var userBankAccounts = context.BankAccounts.Where(x => x.PaymentMethod.User == user);

            foreach (var account in userBankAccounts)
            {
                sb.AppendLine($"-- ID: {account.BankAccountId}");
                sb.AppendLine($"--- Balance: {account.Balance:F2}");
                sb.AppendLine($"--- Bank: {account.BankName}");
                sb.AppendLine($"--- SWIFT: {account.SwiftCode}");
            }

            sb.AppendLine("Creddit Cards:");

            var userCreditCards = context.CreditCards.Where(x => x.PaymentMethod.User == user);

            foreach(var cc in userCreditCards)
            {
                sb.AppendLine($"-- ID: {cc.CreditCardId}");
                sb.AppendLine($"--- Limit: {cc.Limit:F2}");
                sb.AppendLine($"--- Money Owed: {cc.MoneyOwed:F2}");
                sb.AppendLine($"--- Limit Left: {cc.LimitLeft:F2}");
                sb.AppendLine($"--- Expiration Date: {cc.ExpirationDate.ToString()}");
            }

            return sb.ToString();
        }
    }
}
