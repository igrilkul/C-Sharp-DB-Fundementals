using System;
using System.Data.SqlClient;
using ADONET_exercises;

namespace Problem3
{
    class Program
    {
        static void Main(string[] args)
        {
            int id = int.Parse(Console.ReadLine());
            string query = @"SELECT Name FROM Villains WHERE Id = @Id";
            string minionsQuery = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age
                                    FROM MinionsVillains AS mv
                                    JOIN Minions As m ON mv.MinionId = m.Id
                                   WHERE mv.VillainId = @Id
                                ORDER BY m.Name";
            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    string villainName = (string)command.ExecuteScalar();
                    if(villainName==null)
                    {
                        Console.WriteLine($"No villain with ID {id} exists in the database.");
                    }
                    else
                    {
                        Console.WriteLine($"Villain: {villainName}");
                        using (SqlCommand minionsCommand = new SqlCommand(minionsQuery, connection))
                        {
                            minionsCommand.Parameters.AddWithValue("@Id", id);

                            using (SqlDataReader reader = minionsCommand.ExecuteReader())
                            {
                                while(reader.Read())
                                {
                                    long rowNumber = (long)reader[0];
                                    string name = (string)reader[1];
                                    int age = (int)reader[2];

                                    Console.WriteLine($"{rowNumber}. {name} {age}");
                                }

                                if(!reader.HasRows)
                                {
                                    Console.WriteLine("(no minions)");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
