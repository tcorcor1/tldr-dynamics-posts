using System;
using System.IO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace azure_synapse_sqlclient_console
{
    internal class Program
    {
        private static void Main (string[] args)
        {
            #region setup appsettings

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);
            IConfiguration config = builder.Build();

            #endregion setup appsettings

            var dbConn = config.GetConnectionString("DbConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(dbConn))
                {
                    connection.Open();

                    string query = @"SELECT TOP(5) * FROM [dataverse_tldrdynamics_unq3345a000000000000000].[dbo].[yyz_team];";
                    using (SqlCommand sqlCommand = new SqlCommand(query, connection))
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(String.Format("{0}", reader["yyz_team_name"]));
                        };
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }
    }
}