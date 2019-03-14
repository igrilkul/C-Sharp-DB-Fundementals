using System;
using System.Data.SqlClient;
using ADONET_exercises;

namespace Problem6
{
    class Program
    {
        static void Main(string[] args)
        {
            int id = int.Parse(Console.ReadLine());

            string getVillainQuery = @"SELECT Name FROM Villains WHERE Id = @villainId";

            string deletemvQuery = @"DELETE FROM MinionsVillains
      WHERE VillainId = @villainId";

            string deleteVillainQuery = @"DELETE FROM Villains
      WHERE Id = @villainId";

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                string name;

                //get villain name
                using (SqlCommand command = new SqlCommand(getVillainQuery, connection))
                {
                    command.Parameters.AddWithValue("@villainId", id);
                    name = (string)command.ExecuteScalar();
                }

                if(name == null)
                {
                    Console.WriteLine("No such villain was found.");
                }
                else
                {
                    //delete from minionsVillains
                    int rowsAffected;
                    using (SqlCommand command = new SqlCommand(deletemvQuery, connection))
                    {
                        command.Parameters.AddWithValue("@villainId", id);
                        rowsAffected = command.ExecuteNonQuery();
                    }

                    //delete from villains
                    using (SqlCommand command = new SqlCommand(deleteVillainQuery, connection))
                    {
                        command.Parameters.AddWithValue("@villainId", id);
                    }

                    Console.WriteLine($"{name} was deleted.");
                    Console.WriteLine($"{rowsAffected} minions were released.");
                }
               
            }
        }
    }
}
