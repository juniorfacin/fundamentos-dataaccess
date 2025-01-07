using System.ComponentModel.Design;
using System.Data.Common;
using System.Runtime.CompilerServices;
using BaltaDataAccess.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;

namespace BaltaDataAccess
{
    public class Program
    {
        static void Main(string[] args)
        {
            const string stringConnection = "Server=localhost,1433;Database=balta;User ID=sa;Password=1q2w3e4r@#$;Encrypt=True;TrustServerCertificate=True";

            using (var connection = new SqlConnection(stringConnection))
            {
                // CreateCategory(connection);
                // CreateManyCategory(connection);
                // UpdateCategory(connection);
                // DeleteCategory(connection);
                // ListCategories(connection);
                // CreateCategory(connection);
                //GetCategory(connection);
                // ExecuteProcedure(connection);
                // ListStudent(connection);
                // ExecuteReadProcedure(connection);
                // ReadView(connection);
                OneToOne(connection);
            }
            static void ListCategories(SqlConnection connection)
            {
                var categories = connection.Query<Category>("SELECT [Id], [Title] FROM [Category] ORDER BY [Title] ASC");

                foreach (var item in categories)
                {
                    Console.WriteLine($"{item.Id} - {item.Title}");
                }
            }

            static void CreateManyCategory(SqlConnection connection)
            {
                var category = new Category();
                category.Id = Guid.NewGuid();
                category.Title = "Amazon AWS";
                category.Url = "amazon";
                category.Summary = "AWS Cloud";
                category.Description = "Categoria destinada a serviços do AWS";
                category.Order = 8;
                category.Featured = false;

                var category2 = new Category();
                category2.Id = Guid.NewGuid();
                category2.Title = "Categoria Nova";
                category2.Url = "categoria-nova";
                category2.Summary = "Categoria";
                category2.Description = "Categoria nova";
                category2.Order = 9;
                category2.Featured = true;

                var insertSql = @"INSERT INTO 
                [Category] 
            VALUES(
                @Id, 
                @Title, 
                @Url, 
                @Summary, 
                @Order, 
                @Description, 
                @Featured)";

                var rows = connection.Execute(insertSql, new[]{
                new
                {
                     category.Id,
                    category.Title,
                    category.Url,
                    category.Summary,
                    category.Order,
                    category.Description,
                    category.Featured
                }, new
                {
                    category2.Id,
                    category2.Title,
                    category2.Url,
                    category2.Summary,
                    category2.Order,
                    category2.Description,
                    category2.Featured
                }
                });
                Console.WriteLine($"{rows} linhas modificadas.");
            }

            static void UpdateCategory(SqlConnection connection)
            {
                var updateQuery = "UPDATE [Category] SET [Title]=@title WHERE [Id]=@id";
                var rows = connection.Execute(updateQuery, new
                {
                    id = new Guid("af3407aa-11ae-4621-a2ef-2028b85507c4"),
                    title = "Frontend 2024"
                });

                Console.WriteLine($"{rows} registro(s) atualizado(s)");
            }

            // Student Queries
            static void ExecuteProcedure(SqlConnection connection)
            {
                var sql = "spDeleteStudent";
                var pars = new { StudentId = "6f583b1d-e165-46b3-9012-4dd200de6333" };
                var affectedRows = connection.Execute(
                   sql,
                   pars,
                   commandType: System.Data.CommandType.StoredProcedure);

                Console.WriteLine($"{affectedRows} linhas afetadas");
            }

            static void ListStudent(SqlConnection connection)
            {
                var students = connection.Query<Student>("SELECT [Id], [Name], [Email], [Document] FROM [Student] ORDER BY [Name] ASC");

                foreach (var student in students)
                {
                    Console.WriteLine($@" 
                    {student.Id},
                    {student.Name},
                    {student.Email}, 
                    {student.Document}");
                }
            }

            static void ExecuteReadProcedure(SqlConnection connection)
            {
                var procedure = "[spGetCoursesByCategory]";

                var pars = new
                {
                    CategoryId =
                    "af3407aa-11ae-4621-a2ef-2028b85507c4"
                };
                var courses = connection.Query(
                    procedure,
                    pars,
                    commandType: System.Data.CommandType.StoredProcedure);

                foreach (var item in courses)
                {
                    Console.WriteLine($"{item.Id} - {item.Title}");
                }
            }

            static void ReadView(SqlConnection connection)
            {
                var sql = "SELECT * FROM [vwCourses]";
                var courses = connection.Query(sql);

                foreach (var item in courses)
                {
                    Console.WriteLine($"{item.Id} - {item.Title}");

                }
            }

            static void OneToOne(SqlConnection connection)
            {
                var sql = @"
                        SELECT
                            *
                        FROM
                            [CareerItem]
                        INNER JOIN
                            [Course] ON [CareerItem].[CourseId] = [Course].[Id]";

                var items = connection.Query<CareerItem, Course, CareerItem>(
                    sql,
                    (careerItem, course) =>
                    {
                        careerItem.Course = course;
                        return careerItem;
                    }

                );

                foreach (var item in items)
                {
                    Console.WriteLine($"{item.Title} - Curso: {item.Course.Title}");
                }
            }
        }
    }
}