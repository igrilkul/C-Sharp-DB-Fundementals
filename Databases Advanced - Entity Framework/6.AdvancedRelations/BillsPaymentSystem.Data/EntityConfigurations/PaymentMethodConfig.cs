using BillsPaymentSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillsPaymentSystem.Data.EntityConfigurations
{
  public class PaymentMethodConfig : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.HasKey(p => p.PaymentMethodId);

            builder.HasOne(p => p.User)
                .WithMany(u => u.PaymentMethods);

            builder.HasOne(p => p.CreditCard)
                .WithOne(c => c.PaymentMethod);

            builder.HasOne(p => p.BankAccount)
                .WithOne(c => c.PaymentMethod);
        }
    }
}
