using BillsPaymentSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillsPaymentSystem.Data.EntityConfigurations
{
  public  class BankAccountConfig : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder.HasKey(b => b.BankAccountId);

            builder.Property(u => u.BankName)
                .HasMaxLength(50).IsUnicode()
                .IsRequired();
            

            builder.Property(u => u.SwiftCode)
                .HasMaxLength(20).IsUnicode(false)
                .IsRequired();
            
        }
    }
}
