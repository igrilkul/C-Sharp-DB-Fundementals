using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ADONET_exercises;
namespace Problem5
{
   public class Program
    {
        static void Main(string[] args)
        {
          string updateTownsQuery =  @"UPDATE Towns
   SET Name = UPPER(Name)
 WHERE CountryCode = (SELECT c.Id FROM Countries AS c WHERE c.Name = @countryName)";

          string getTownsQuery = @"SELECT t.Name
   FROM Towns as t
   JOIN Countries AS c ON c.Id = t.CountryCode
  WHERE c.Name = @countryName";

            string country = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(updateTownsQuery, connection))
                {
                    command.Parameters.AddWithValue("@countryName", country);
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"{rowsAffected} town names were affected.");
                }

                List<string> towns = new List<string>();

                using (SqlCommand command = new SqlCommand(getTownsQuery, connection))
                {
                    command.Parameters.AddWithValue("@countryName", country);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            towns.Add((string)reader[0]);
                        }
                        Console.WriteLine($"[{string.Join(", ",towns)}]");
                    }
                }
            }
        }
    }
}
