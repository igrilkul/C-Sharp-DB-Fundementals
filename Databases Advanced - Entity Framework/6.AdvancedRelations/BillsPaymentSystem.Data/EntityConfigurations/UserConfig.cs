using BillsPaymentSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillsPaymentSystem.Data.EntityConfigurations
{
   public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.UserId);

            

            builder.HasMany(u => u.PaymentMethods)
                .WithOne(c => c.User);

            builder.Property(u => u.FirstName)
                .HasMaxLength(50).IsUnicode()
                .IsRequired();

            builder.Property(u => u.LastName)
                .HasMaxLength(50).IsUnicode()
                .IsRequired();

            builder.Property(u => u.Email)
                .HasMaxLength(80).IsUnicode(false)
                .IsRequired();
            

            builder.Property(u => u.Password)
                .HasMaxLength(25).IsUnicode(false)
                .IsRequired();
            
        }
    }
}
