using System;
using System.Data.SqlClient;

namespace _03.MinionNames
{
    class StartUp
    {
        private static string connectionString =
            "Server=LAPTOP-SELJOP4P\\SQLEXPRESS;" +
            "Database=MinionsDB;" +
            "Integrated Security=true";

        static void Main(string[] args)
        {
            int villainId = int.Parse(Console.ReadLine());

            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();

            using (connection)
            {
                string queryText = @"SELECT Name FROM Villains WHERE Id = @Id";

                SqlCommand command = new SqlCommand(queryText, connection);
                command.Parameters.AddWithValue("@Id", villainId);

                object value = command.ExecuteScalar();

                if (value == null)
                {
                    Console.WriteLine($"No villain with ID {villainId} exists in the database.");
                    return;
                }

                string villianName = (string)value;

                queryText = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age
                                    FROM MinionsVillains AS mv
                                    JOIN Minions As m ON mv.MinionId = m.Id
                                   WHERE mv.VillainId = @Id
                                ORDER BY m.Name";

                command = new SqlCommand(queryText, connection);

                SqlDataReader reader = command.ExecuteReader();

                using (reader)
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("(no minions)");
                        reader.Close();
                        connection.Close();
                        return;

                    }
                    while (reader.Read())
                    {
                        string minionName = (string)reader["Name"];
                        int rowNum = (int)reader["RowNum"];
                        int minionAge = (int)reader["Age"];

                        Console.WriteLine($"{rowNum}. {minionName} {minionAge}");
                    }
                }
            }

        }
    }
}
