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
                // OneToOne(connection);
                // OneToMany(connection);
                // QueryMutiple(connection);
                // Like(connection, "frontend");
                Transaction(connection);

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
                    }, splitOn: "Id");

                foreach (var item in items)
                {
                    Console.WriteLine($"{item.Title} - Curso: {item.Course.Title}");
                }
            }

            static void OneToMany(SqlConnection connection)
            {
                var sql = @"
                SELECT 
                    [Career].[Id],
                    [Career].[Title],
                    [CareerItem].[CareerId],
                    [CareerItem].[Title]
                FROM 
                    [Career] 
                INNER JOIN 
                    [CareerItem] ON [CareerItem].[CareerId] = [Career].[Id]
                ORDER BY
                    [Career].[Title]";

                var careers = new List<Career>();
                var items = connection.Query<Career, CareerItem, Career>(
                    sql,
                    (career, item) =>
                    {
                        var car = careers.Where(x => x.Id == career.Id).FirstOrDefault();
                        if (car == null)
                        {
                            car = career;
                            car.Items.Add(item);
                            careers.Add(car);
                        }
                        else
                        {
                            car.Items.Add(item);
                        }

                        return career;
                    }, splitOn: "CareerId");

                foreach (var career in careers)
                {
                    Console.WriteLine($"{career.Title}");
                    foreach (var item in career.Items)
                    {
                        Console.WriteLine($" - {item.Title}");
                    }
                }
            }

            static void QueryMutiple(SqlConnection connection)
            {
                var query = "SELECT * FROM [Category]; SELECT * FROM [Course]";

                using (var multi = connection.QueryMultiple(query))
                {
                    var categories = multi.Read<Category>();
                    var courses = multi.Read<Course>();

                    foreach (var item in categories)
                    {
                        Console.WriteLine(item.Title);
                    }

                    foreach (var item in courses)
                    {
                        Console.WriteLine(item.Title);
                    }
                }
            }

            static void SelectIn(SqlConnection connection)
            {
                var query = @"select * from Career where [Id] IN @Id";

                var items = connection.Query<Career>(query, new
                {
                    Id = new[]{
                    "4327ac7e-963b-4893-9f31-9a3b28a4e72b",
                    "e6730d1c-6870-4df3-ae68-438624e04c72"
                }
                });

                foreach (var item in items)
                {
                    Console.WriteLine(item.Title);
                }
            }

            static void Like(SqlConnection connection, string term)
            {
                var query = @"SELECT * FROM [Course] WHERE [Title] LIKE @exp";

                var items = connection.Query<Course>(query, new
                {
                    exp = $"%{term}%"
                });

                foreach (var item in items)
                {
                    Console.WriteLine(item.Title);
                }
            }

            static void Transaction(SqlConnection connection)
            {
                var category = new Category();
                category.Id = Guid.NewGuid();
                category.Title = "Minha categoria que não";
                category.Url = "amazon";
                category.Description = "Categoria destinada a serviços do AWS";
                category.Order = 8;
                category.Summary = "AWS Cloud";
                category.Featured = false;

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

                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var rows = connection.Execute(insertSql, new
                    {
                        category.Id,
                        category.Title,
                        category.Url,
                        category.Summary,
                        category.Order,
                        category.Description,
                        category.Featured
                    }, transaction);

                    transaction.Commit();
                    // transaction.Rollback();
                    Console.WriteLine($"{rows} linhas inseridas");
                }
            }
        }
    }
}