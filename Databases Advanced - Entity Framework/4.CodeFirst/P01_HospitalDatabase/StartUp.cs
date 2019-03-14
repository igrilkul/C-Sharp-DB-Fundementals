using System;
using P01_HospitalDatabase.Data;

namespace P01_HospitalDatabase
{
   public class StartUp
    {
        static void Main(string[] args)
        {
            //Creating the DB

            //Method A
            //var db = new HospitalContext();
            //db.Database.EnsureCreated();

            //Method B
            //Open Tools -> Package manager console
            //Default Project : ...Data

            //Install:
            //Microsoft.EntityFrameworkCore v2.2.0
            //Microsoft.EntityFrameworkCore.SqlServer v2.2.0
            //Microsoft.EntityFrameworkCore.SqlServer.Design v1.1.6
            //Microsoft.EntityFrameworkCore.Tools v2.2.0

            //adding the migration:
            //open tools -> nuget manager console
            //Add-Migration InitialCreate (make sure proper project is default project

            //NOTE: Judge does NOT accept Tools and Design packagess installed

        }
    }
}
