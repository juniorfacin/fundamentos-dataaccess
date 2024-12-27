using System.Data.Common;
using BaltaDataAccess.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BaltaDataAccess
{
    public class Program
    {
        static void Main(string[] args)
        {
            const string stringConnection = "Server=localhost,1433;Database=balta;User ID=sa;Password=1q2w3e4r@#$;Encrypt=True;TrustServerCertificate=True";

            using (var connection = new SqlConnection(stringConnection))
            {
                Console.WriteLine("Conectado");
                var categories = connection.Query<Category>("SELECT [Id], [Title] FROM [Category]");

                foreach (var category in categories)
                {
                    Console.WriteLine($"{category.Id} - {category.Title}");
                }
            }
        }
    }
}