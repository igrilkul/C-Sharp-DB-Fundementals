using Microsoft.EntityFrameworkCore;
using MyApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Data
{
    public class MyAppContext : DbContext
    {
        public MyAppContext(DbContextOptions options):base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
    }
}
