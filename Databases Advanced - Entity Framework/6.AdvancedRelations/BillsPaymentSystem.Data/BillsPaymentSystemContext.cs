using BillsPaymentSystem.Data.EntityConfigurations;
using BillsPaymentSystem.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;

namespace BillsPaymentSystem.Data
{
    public class BillsPaymentSystemContext:DbContext
    {
        public BillsPaymentSystemContext( DbContextOptions options) 
            : base(options)
        {
        }

        public BillsPaymentSystemContext()
        {
        }

        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Config.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.ApplyConfiguration(new BankAccountConfig());
            mb.ApplyConfiguration(new CreditCardConfig());
            mb.ApplyConfiguration(new PaymentMethodConfig());
            mb.ApplyConfiguration(new UserConfig());
        }
    }
}
