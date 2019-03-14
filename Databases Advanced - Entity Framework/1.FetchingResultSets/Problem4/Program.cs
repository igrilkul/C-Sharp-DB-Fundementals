using System;
using System.Data.SqlClient;
using ADONET_exercises;

//Transaction part commented - not working properly
namespace Problem4
{
   public class Program
    {
        static void Main(string[] args)
        {
            string[] minionInput = Console.ReadLine().Split();
            string mName = minionInput[1];
            int mAge = int.Parse(minionInput[2]);
            string mTown = minionInput[3];

            string[] villainInput = Console.ReadLine().Split(": ");
            string vName = villainInput[1];

            string townCheckQuery = @"SELECT Id FROM Towns WHERE Name = @townName";
            string villainCheckQuery = @"SELECT Id FROM Villains WHERE Name = @villainName";
            string minionCheckQuery = @"SELECT Id FROM Minions WHERE Name = @Name";

            string townInsertQuery = @"INSERT INTO Towns (Name) VALUES (@townName)";
            string villainInsertQuery = @"INSERT INTO Villains (Name, EvilnessFactorId)  VALUES (@villainName, 4)";
            

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                //SqlTransaction transaction = null;
                //try
                //{
                    connection.Open();
                    //transaction = connection.BeginTransaction();
                    string staticTownParam = "@townName";
                    int? townId = GetCheckId(staticTownParam, mTown, townCheckQuery, connection);

                    if (townId == null)
                    {
                        //Add Town
                        AddTownOrVillain(mTown, townInsertQuery, connection, staticTownParam);

                        townId = GetCheckId(staticTownParam, mTown, townCheckQuery, connection);

                        Console.WriteLine($"Town {mTown} was added to the database.");
                    }

                    string staticVillainParam = "@villainName";
                    int? villainId = GetCheckId(staticVillainParam, vName, villainCheckQuery, connection);

                    if (villainId == null)
                    {
                        //Add Villain
                        AddTownOrVillain(vName, villainInsertQuery, connection, staticVillainParam);

                        villainId = GetCheckId(staticVillainParam, vName, villainCheckQuery, connection);

                        Console.WriteLine($"Villain {vName} was added to the database.");
                    }

                    AddMinion(connection, mName, mAge, townId);

                    string staticMinionParam = "@Name";
                    int? minionId = GetCheckId(staticMinionParam, mName, minionCheckQuery, connection);

                    AddMinionVillain(connection, villainId, minionId);

                    Console.WriteLine($"Successfully added {mName} to be minion of {vName}.");
                    //transaction.Commit();
                    //Console.WriteLine("Changes succesfully commited");
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine($"Commit Exception Type: {ex.GetType()}");
                //    Console.WriteLine($"    Message: {ex.Message}");

                //    //Attempt to rollback
                //    try
                //    {
                //        Console.WriteLine("Something went wrong! Attempting DB Rollback.");
                //        transaction.Rollback();
                //        Console.WriteLine("Rollback successful.");
                //    }
                //    catch(Exception ex2)
                //    {
                //        Console.WriteLine($"Rollback Exception Type: {ex2.GetType()}");
                //        Console.WriteLine($"    Message: {ex2.Message}");
                //        Console.WriteLine("Transaction rollback failed!");
                //    }
                //}
                
            }
        }

        private static void AddMinionVillain(SqlConnection connection, int? villainId, int? minionId)
        {
            string minionVillainInsertQuery = @"INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@minionId, @villainId)";
            using (SqlCommand command = new SqlCommand(minionVillainInsertQuery, connection))
            {
                command.Parameters.AddWithValue("@villainId", villainId);
                command.Parameters.AddWithValue("@minionId", minionId);
                command.ExecuteNonQuery();
            }
        }

        private static void AddMinion(SqlConnection connection, string mName, int mAge, int? townId)
        {
            string minionInsertQuery = "INSERT INTO Minions (Name, Age, TownId) VALUES (@name, @age, @townId)";

            using (SqlCommand command = new SqlCommand(minionInsertQuery, connection))
            {
                command.Parameters.AddWithValue("@name",mName);
                command.Parameters.AddWithValue("@age",mAge);
                command.Parameters.AddWithValue("@townId",townId);
                command.ExecuteNonQuery();
            }
        }

        private static void AddTownOrVillain(string param, string cmdText, SqlConnection connection,string staticParam)
        {
            using (SqlCommand townCommand = new SqlCommand(cmdText, connection))
            {
                townCommand.Parameters.AddWithValue(staticParam, param);
                townCommand.ExecuteScalar();
            }
        }

        private static int? GetCheckId(string staticParam, string param, string query, SqlConnection connection)
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue(staticParam, param);
                int? id = (int?)command.ExecuteScalar();
                return id;
            }
        }
    }
}
