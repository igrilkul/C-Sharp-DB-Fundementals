using Microsoft.EntityFrameworkCore;
using P03_SalesDatabase.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using P03_SalesDatabase.Data;

namespace P03_SalesDatabase.Data
{
    public class SalesContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Sale> Sales { get; set; }

        public DbSet<Store> Stores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Configure DB
            optionsBuilder.UseSqlServer(Config.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            //Constraints and relations
            ConfigureProductEntity(mb);
            ConfigureCustomerEntity(mb);
            ConfigureStoreEntity(mb);
            ConfigureSaleEntity(mb);
        }

        private void ConfigureSaleEntity(ModelBuilder mb)
        {
            mb.Entity<Sale>()
                .HasKey(s => s.SaleId);

            mb.Entity<Sale>()
                .Property(s => s.Date)
                .HasDefaultValueSql("GETDATE()");
        }

        private void ConfigureStoreEntity(ModelBuilder mb)
        {
            mb.Entity<Store>()
                .HasKey(s => s.StoreId);

            mb.Entity<Store>()
                .Property(s => s.Name)
                .HasMaxLength(80)
                .IsUnicode();

            mb.Entity<Store>()
                .HasMany(s => s.Sales)
                .WithOne(t => t.Store);
        }

        private void ConfigureCustomerEntity(ModelBuilder mb)
        {
            mb.Entity<Customer>()
                .HasKey(c => c.CustomerId);

            mb.Entity<Customer>()
                .Property(c => c.Name)
                .HasMaxLength(100)
                .IsUnicode();

            mb.Entity<Customer>()
               .Property(c => c.Email)
               .HasMaxLength(80)
               .IsUnicode(false);

            mb.Entity<Customer>()
                .HasMany(c => c.Sales)
                .WithOne(s => s.Customer);
        }

        private void ConfigureProductEntity(ModelBuilder mb)
        {
            mb
                .Entity<Product>()
                .HasKey(p => p.ProductId);

            mb
                .Entity<Product>()
                .Property(p => p.Name)
                .HasMaxLength(50)
                .IsUnicode();

            mb
                .Entity<Product>()
                .HasMany(p => p.Sales)
                .WithOne(s => s.Product);

            mb
                .Entity<Product>()
                .Property(p => p.Description)
                .HasMaxLength(250)
                .HasDefaultValue("No description");
        }
    }
}