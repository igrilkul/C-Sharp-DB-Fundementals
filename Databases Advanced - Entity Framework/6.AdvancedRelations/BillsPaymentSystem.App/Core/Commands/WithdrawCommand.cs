using BillsPaymentSystem.App.Core.Commands.Contracts;
using BillsPaymentSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BillsPaymentSystem.App.Core.Commands
{
    public class WithdrawCommand : ICommand
    {
        private readonly BillsPaymentSystemContext context;

        public WithdrawCommand(BillsPaymentSystemContext context)
        {
            this.context = context;
        }

        public string Execute(string[] args)
        {
            int userId = int.Parse(args[0]);
            decimal amount = decimal.Parse(args[1]);

          var result =  Withdraw(userId, amount);
            return result;
        }

        private string Withdraw(int userId, decimal amount)
        {
            var totalBankMoney = this.context.PaymentMethods.Where(x => x.UserId == userId).Sum(y => y.BankAccount.Balance);

                var totalCCMoney = this.context.PaymentMethods.Where(x => x.UserId == userId).Where(c=>c.CreditCard!=null).Sum(y => y.CreditCard.LimitLeft);

            if(amount>totalBankMoney+totalCCMoney)
            {
                throw new ArgumentException("Not enough money!");
            }
            else
            {
                decimal copyOfAmount = amount;

                var accounts = this.context.PaymentMethods.Where(u => u.UserId == userId).Where(b=>b.BankAccount!=null).Select(x => x.BankAccount).OrderBy(i => i.BankAccountId);

                foreach (var account in accounts)
                {
                    if(account.Balance<amount)
                    {
                        amount -= account.Balance;
                        account.Balance = 0;
                    }
                    else
                    {
                        account.Balance -= amount;
                        amount = 0;
                        break;
                    }
                }

                if(amount>0)
                {
                    var cards = this.context.PaymentMethods.Where(u => u.UserId == userId).Where(c=>c.CreditCard!=null).Select(x => x.CreditCard).OrderBy(i => i.CreditCardId);

                    foreach (var card in cards)
                    {
                        if (card.LimitLeft < amount)
                        {
                            amount -= card.LimitLeft;
                            card.MoneyOwed = card.Limit;
                        }
                        else
                        {
                            card.MoneyOwed += amount;
                            amount = 0;
                            break;
                        }
                    }

                }
                context.SaveChanges();
                string result = $"${copyOfAmount} Withdrawn successfully.";
                return result;
            }
        }
    }
}
