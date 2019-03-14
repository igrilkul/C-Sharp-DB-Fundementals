using System;
using ADONET_exercises;
using System.Data.SqlClient;

namespace Problem9
{
    class Program
    {
        static void Main(string[] args)
        {
            int id = int.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string proc = @"usp_GetOlder";
                using (SqlCommand command = new SqlCommand(proc, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }

                string selectQuery = @"SELECT Name, Age FROM Minions WHERE Id = @Id";
                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            string name = (string)reader[0];
                            int age = (int)reader[1];

                            Console.WriteLine($"{name} - {age} years old");
                        }
                    }
                }
            }
        }
    }
}
