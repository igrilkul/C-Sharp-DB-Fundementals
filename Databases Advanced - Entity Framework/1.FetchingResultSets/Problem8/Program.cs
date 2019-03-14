using System;
using ADONET_exercises;
using System.Data.SqlClient;
using System.Linq;

namespace Problem8
{
    class Program
    {
        static void Main(string[] args)
        {
          string updateQuery = @"UPDATE Minions
   SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1
 WHERE Id = @Id";

           string selectQuery = @"SELECT Name, Age FROM Minions";

            int[] ids = Console.ReadLine().Split().Select(int.Parse).ToArray();

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                for (int i=0;i<ids.Length;i++)
                {
                    using (SqlCommand command = new SqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", ids[i]);
                        command.ExecuteNonQuery();
                    }
                }

                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            string name = (string)reader[0];
                            int age = (int)reader[1];

                            Console.WriteLine($"{name} {age}");
                        }
                    }
                }
            }
        }
    }
}
