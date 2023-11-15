using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace _07.MinionNames
{
    class StartUp
    {
        private static string connectionString =
             "Server=LAPTOP-SELJOP4P\\SQLEXPRESS;" +
             "Database=MinionsDB;" +
             "Integrated Security=true";
        static void Main(string[] args)
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;" + "Integrated Security=true;" + "initial catalog=MinionsDB";

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            List<string> minionsInitial = new List<string>();
            List<string> minionsArranged = new List<string>();

            using (connection)
            {
                SqlCommand command = new SqlCommand("SELECT Name FROM Minions", connection);

                SqlDataReader reader = command.ExecuteReader();

                using (reader)
                {
                    if (!reader.HasRows)
                    {
                        reader.Close();
                        connection.Close();
                        return;
                    }

                    while (reader.Read())
                    {
                        minionsInitial.Add((string)reader["Name"]);
                    }
                }
            }

            while (minionsInitial.Count > 0)
            {
                minionsArranged.Add(minionsInitial[0]);
                minionsInitial.RemoveAt(0);

                if (minionsInitial.Count > 0)
                {
                    minionsArranged.Add(minionsInitial[minionsInitial.Count - 1]);
                    minionsInitial.RemoveAt(minionsInitial.Count - 1);
                }
            }

            minionsArranged.ForEach(m => Console.WriteLine(m));

        }
    }
}
