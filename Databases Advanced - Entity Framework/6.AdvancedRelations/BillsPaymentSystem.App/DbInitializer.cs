using BillsPaymentSystem.Data;
using BillsPaymentSystem.Models;
using BillsPaymentSystem.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BillsPaymentSystem.App
{
   public class DbInitializer
    {
        private readonly BillsPaymentSystemContext context;
        public DbInitializer(BillsPaymentSystemContext context)
        {
            this.context = context;
        }

        public void InitializeDB(BillsPaymentSystemContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            Seed(context);
        }
        public static void Seed(BillsPaymentSystemContext context)
        {
            
            SeedUsers(context);
            SeedCreditCards(context);
            SeedBankAccounts(context);
            SeedPaymentMethods(context);
        }

        private static void SeedPaymentMethods(BillsPaymentSystemContext context)
        {
            var paymentMethods = new List<PaymentMethod>();

            for(int i=0;i<15;i++)
            {
                var paymentMethod = new PaymentMethod
                {
                    UserId = new Random().Next(1, 5),
                    PaymentType = (PaymentType)new Random().Next(0, 2),

                };

                if(i%3==0)
                {
                    paymentMethod.CreditCardId = 1;
                    paymentMethod.BankAccountId = 1;
                }
                else if(i % 2 == 0)
                {
                    paymentMethod.CreditCardId = i;
                    paymentMethod.BankAccountId = null;
                }
                else
                {
                    paymentMethod.BankAccountId = i;
                    paymentMethod.CreditCardId = null;
                }

                if (!IsValid(paymentMethod))
                {
                    continue;
                }

                if(paymentMethod.CreditCardId==null && paymentMethod.BankAccountId==null)
                {
                    Console.WriteLine("ERROR");
                    continue;
                }

                var user = context.Users.Find(paymentMethod.UserId);
                var creditCard = context.CreditCards.Find(paymentMethod.CreditCardId);
                var bankAccount = context.BankAccounts.Find(paymentMethod.BankAccountId);

                if(user == null || (creditCard==null && bankAccount==null))
                {
                    continue;
                }

                paymentMethods.Add(paymentMethod);
            }

            context.AddRange(paymentMethods);
            context.SaveChanges();
        }

        private static void SeedBankAccounts(BillsPaymentSystemContext context)
        {
            var bankAccounts = new List<BankAccount>();

            for(int i=0;i<100;i++)
            {
                var bankAccount = new BankAccount()
                {
                    Balance = new Random().Next(-200, 200),
                    BankName = "Banka " + i,
                    SwiftCode = "Swift " + i
                };

                if(!IsValid(bankAccount))
                {
                    continue;
                }
                bankAccounts.Add(bankAccount);
            }

            context.AddRange(bankAccounts);
            context.SaveChanges();
        }

        private static void SeedCreditCards(BillsPaymentSystemContext context)
        {
            var creditCards = new List<CreditCard>();

            for(int i = 0; i < 100; i++)
            {
                var creditCard = new CreditCard
                {
                    Limit = new Random().Next(-5000, 25000),
                    MoneyOwed = new Random().Next(-5000, 25000),
                    ExpirationDate = DateTime.Now.AddDays(new Random().Next(-10, 100))
                };

                if(!IsValid(creditCard))
                {
                    continue;
                }

                creditCards.Add(creditCard);
            }

            context.CreditCards.AddRange(creditCards);
            context.SaveChanges();
        }

        private static void SeedUsers(BillsPaymentSystemContext context)
        {
            string[] firstNames = { "Ivar", "Ragnar", "Floki","Lagarta", null, "" };
            string[] lastNames = { "Haah","FOOOls","GMAIL","Hromma", null, "" };
            string[] emails = { "gosho@abv.bg","RagnarLothbrok@abv.bg","vallhalla@abv.bg","HroMma@abv.bg", null, "" };
            string[] passwords = { "123456", "password", "12345678","strongPass", null, "ERROR" };

            List<User> users = new List<User>();

            for(int i=0;i<firstNames.Length;i++)
            {
                if(emails[i]==null)
                {
                    continue;
                }

                var user = new User
                {
                    FirstName = firstNames[i],
                    LastName = lastNames[i],
                    Email = emails[i].ToLower(),
                    Password = passwords[i]
                };

                if(!IsValid(user))
                {
                    continue;
                }
                users.Add(user);
            }

            context.Users.AddRange(users);
            context.SaveChanges();
        }

        private static bool IsValid(object entity)
        {
            var validationContext = new ValidationContext(entity);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(entity, validationContext, validationResults, true);

            return isValid;
        }
    }
}
