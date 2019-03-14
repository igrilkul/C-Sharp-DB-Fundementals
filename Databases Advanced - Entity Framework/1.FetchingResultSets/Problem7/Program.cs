using System;
using ADONET_exercises;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Problem7
{
    class Program
    {
        static void Main(string[] args)
        {
            string query = @"SELECT Name FROM Minions";

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                List<String> minons = new List<string>();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            minons.Add((string)reader[0]);
                        }
                        int a = 0;
                        int b = minons.Count-1;
                        while(a!=b)
                        {
                            //kept the id's for easier testing
                            Console.WriteLine($"{minons[a]} {a}");
                            Console.WriteLine($"{minons[b]} {b}");
                            a++;
                            b--;
                        }
                        if(minons.Count%2!=0)
                        {
                            Console.WriteLine($"{minons[a]} {a}");
                        }
                        
                    }
                }
            }
        }
    }
}
