using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Api.Models;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
namespace Api
{

    public static class UserFunction
    {
        [FunctionName(nameof(UserAuthenication))]
        public static async Task < IActionResult > UserAuthenication(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth")] HttpRequest req, ILogger log) {
            log.LogInformation("C# HTTP trigger function processed a request");
            
            // TODO: Perform custom authentication here; we're just using a simple hard coded check for this example
            bool authenticated = false;
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();  
            var input = JsonConvert.DeserializeObject<UserCredentials>(requestBody); 
            User authUser = new User();
            var appsettingvalue = "";
            try
            {
                appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");
                using (SqlConnection conn = new SqlConnection(appsettingvalue))  
                {
                    conn.Open();  
                    if(!String.IsNullOrEmpty(input.username))  
                    {  
                        Console.WriteLine(input.username);
                        
                        
                        var query = $"select * from login where username='{input.username}' and password='{input.password}'";  
                        SqlCommand command = new SqlCommand(query, conn);
                        
                        var reader = await command.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            var foundUsername = reader["username"].ToString();
                            if (!string.IsNullOrWhiteSpace(foundUsername))
                            {
                                authenticated = true;
                                authUser.Username = foundUsername;
                                authUser.Email = reader["email"].ToString();
                            }
                        }
                    }  
                }  
            }  
            catch (Exception e)  
            {  
                log.LogError(e.ToString());  
                Console.WriteLine(e.ToString()); 
            }
            
            
            // TODO: Perform custom authentication here; we're just using a simple hard coded check for this example

            
            if (!authenticated) {
                return await Task.FromResult(new UnauthorizedResult()).ConfigureAwait(false);
            } else {
                GenerateJWTToken generateJWTToken = new();

                Guid id = Guid.NewGuid();
            string sessionToken = id.ToString();

            using (SqlConnection conn = new SqlConnection(appsettingvalue))
            {
                conn.Open();
                var query = $"UPDATE login SET sessionToken = '{sessionToken}' WHERE username = '{input.username}'";  
                SqlCommand command = new SqlCommand(query, conn);
                        
                await command.ExecuteReaderAsync();
                
            }

            string token = generateJWTToken.IssuingJWT(input.username, sessionToken);
            authUser.SessionToken = token;
            return await Task.FromResult(new OkObjectResult(authUser)).ConfigureAwait(false);
            }
        }

        [FunctionName("AddFavorite")]
        public static async Task<IActionResult> AddFavorite(ExecutionContext context,
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "favorites")]
            HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();  
            var input = JsonConvert.DeserializeObject<FavoriteEntity>(requestBody);
            try
            {
                var appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");
                using (SqlConnection conn = new SqlConnection(appsettingvalue))  
                {
                    conn.Open();  
                    if(!String.IsNullOrEmpty(input.username))  
                    {
                        var query = $"INSERT INTO [favorites] (username,movie_id) VALUES('{input.username}', '{input.movie_id}')";  
                        SqlCommand command = new SqlCommand(query, conn);  
                        command.ExecuteNonQuery();  
                    }  
                }  
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return new OkResult(); 
        }
        
        [FunctionName("GetFavorites")]
        public static async Task<IActionResult> GetFavorites(ExecutionContext context,
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "favorites/{username}/{movie_id}")]
            HttpRequest req, ILogger log, string username, int movie_id)
        {
            string foundFavorited = "";
            string exists;

            var appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");
            
            using (SqlConnection conn = new SqlConnection(appsettingvalue))
            {
                conn.Open();
                var query = @"select * from favorites where username = @username and movie_id = @movie_id";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@movie_id", movie_id);
                var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    foundFavorited = reader["username"].ToString();
                }
                if (string.IsNullOrWhiteSpace(foundFavorited))
                {
                    exists = "false";
                }
                else
                {
                    exists = "true";
                }
                
            }
            return new OkObjectResult(exists);
        }

        [FunctionName("GetAllFavorites")]
        public static async Task<IActionResult> GetAllFavorites(ExecutionContext context,
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "favorites/{username}")]
            HttpRequest req, ILogger log, string username)
        {
            List<string> foundMovies = new List<string>();

            var appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");
            
            using (SqlConnection conn = new SqlConnection(appsettingvalue))
            {
                conn.Open();
                var query = @"select movie_id from favorites where username = @username";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@username", username);
                var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    foundMovies.Add(reader["movie_id"].ToString()) ;
                }
            }
            return new OkObjectResult(foundMovies);
        }
        
        [FunctionName("GetFavoriteCount")]
        public static async Task<IActionResult> GetFavoriteCount(ExecutionContext context,
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "favoriteCount/{movie_id}")]
            HttpRequest req, ILogger log, int movie_id)
        {
            string count="";

            var appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");
            
            using (SqlConnection conn = new SqlConnection(appsettingvalue))
            {
                conn.Open();
                var query = @"select count(*) from favorites where movie_id = @movie_id";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@movie_id", movie_id);
                var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    count = reader[""].ToString();
                    Console.WriteLine(count);
                }
            }
            return new OkObjectResult(count);
        }
        
        [FunctionName("GetAllUsersFavorite")]
        public static async Task<IActionResult> GetAllUsersFavorite(ExecutionContext context,
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "allFavorite/{var}")]
            HttpRequest req, ILogger log)
        {
            List<string> MostFavorited = new List<string>();

            var appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");
            
            using (SqlConnection conn = new SqlConnection(appsettingvalue))
            {
                conn.Open();
                var query = @"select top 20 movie_id, count(movie_id) as c from favorites group by movie_id order by c DESC";
                SqlCommand command = new SqlCommand(query, conn);
                var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    MostFavorited.Add(reader["movie_id"].ToString());
                }
            }
            return new OkObjectResult(MostFavorited);
        }
        
        [FunctionName("DeleteFavorites")]
        public static async Task<IActionResult> DeleteFavorites(ExecutionContext context,
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "favorites/{username}/{movie_id}")]
            HttpRequest req, ILogger log, string username, int movie_id)
        {
            
            string foundFavorited = "";
            string exists;
            
            var appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");
            
            using (SqlConnection conn = new SqlConnection(appsettingvalue))
            {
                conn.Open();
                var query = @"delete from favorites where username = @username and movie_id = @movie_id";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@movie_id", movie_id);
                var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    foundFavorited = reader["username"].ToString();
                }
                if (string.IsNullOrWhiteSpace(foundFavorited))
                {
                    exists = "false";
                }
                else
                {
                    exists = "true";
                }
                
            }
            return new OkObjectResult(exists);  
        }

        [FunctionName("CreateUser")]  
        public static async Task<IActionResult> CreateUser(ExecutionContext context,
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "signup")] HttpRequest req, ILogger log)  
        {  
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();  
            var input = JsonConvert.DeserializeObject<User>(requestBody);  
            try
            {
                var appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");
                using (SqlConnection conn = new SqlConnection(appsettingvalue))  
                {
                    conn.Open();  
                    if(!String.IsNullOrEmpty(input.Username))  
                    {  
                        Console.WriteLine(input.Email +" EMAIL");
                        Console.WriteLine(input.Username +" USERNAME");
                        
                        var query = $"INSERT INTO [login] (username,password,email) VALUES('{input.Username}', '{input.Password}' , '{input.Email}')";  
                        SqlCommand command = new SqlCommand(query, conn);  
                        command.ExecuteNonQuery();   
                    }  
                }  
            }  
            catch (Exception e)  
            {  
                log.LogError(e.ToString());  
                return new BadRequestResult();  
            }  
            return new OkResult();  
            
        }

        [FunctionName("GetUserByUsername")]
        public static async Task<IActionResult> Run(ExecutionContext context,
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/{username}")]
            HttpRequest req, ILogger log, string username)
        {
            string foundUsername = "";
            string exists;

            var appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");
            
            using (SqlConnection conn = new SqlConnection(appsettingvalue))
            {
                conn.Open();
                var query = @"select * from login where username = @username";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@username", username);
                var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {

                    foundUsername = reader["username"].ToString();

                }

                if (string.IsNullOrWhiteSpace(foundUsername))
                {
                    exists = "false";
                }
                else
                {
                    exists = "true";
                }


            }
            Console.WriteLine(username);
            return new OkObjectResult(exists);
            
        }
        [FunctionName(nameof(GetUserData))]
        public static async Task<IActionResult> GetUserData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "data")] HttpRequest req,
            ILogger log)
        {
            // Check if we have authentication info.
            ValidateJWT auth = new ValidateJWT(req);

            if (!auth.IsValid)
            {
                return new UnauthorizedResult(); // No authentication info.
            }

            string postData = await req.ReadAsStringAsync();

            return new OkObjectResult($"{postData}");

        }
        public static string GetSqlAzureConnectionString(string name)
        {
            string conStr = Environment.GetEnvironmentVariable($"ConnectionStrings:{name}", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(conStr)) // Azure Functions App Service naming convention
                conStr = Environment.GetEnvironmentVariable($"{name}", EnvironmentVariableTarget.Process);
            return conStr;
        }
    }
    
}