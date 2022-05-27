using System;
using System.IO;
using System.Threading.Tasks;
using Api.Models;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
namespace Api
{

    public static class UserFunction
    {
        [FunctionName(nameof(UserAuthenication))]
        public static async Task < IActionResult > UserAuthenication(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth")] HttpRequest req, ILogger log) {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            // TODO: Perform custom authentication here; we're just using a simple hard coded check for this example
            string foundUsername = "";
            bool authenticated = false;
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();  
            var input = JsonConvert.DeserializeObject<UserCredentials>(requestBody); 
            User authUser = new User();
            try
            {
                var appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");
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

                            foundUsername = reader["username"].ToString();
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
                string token = generateJWTToken.IssuingJWT(input.username);
                authUser.Token = token;
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
                        var query = $"INSERT INTO [favorites] (username,movie_id,favorite) VALUES('{input.username}', '{input.movie_id}' , '{input.favorite}')";  
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
            HttpRequest req, ILogger log, string username, string movie_id)
        {
            string foundFavorited = "";
            string exists = "";

            var appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");
            
            using (SqlConnection conn = new SqlConnection(appsettingvalue))
            {
                conn.Open();
                var query = @"select * from favorites where username = @username and movie_id = @movie_id";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@movie_id", Int32.Parse(movie_id));
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
            Console.WriteLine(username);
            return new OkObjectResult(exists);
        }
        
        //TODO change to DELETE
        [FunctionName("DeleteFavorite")]
        public static async Task<IActionResult> UpdateFavorite(ExecutionContext context,
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "favorites/{username}/{movie_id}")]
            HttpRequest req, ILogger log)
        {
            return null;
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
            string exists = "";

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
            string conStr = System.Environment.GetEnvironmentVariable($"ConnectionStrings:{name}", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(conStr)) // Azure Functions App Service naming convention
                conStr = System.Environment.GetEnvironmentVariable($"{name}", EnvironmentVariableTarget.Process);
            return conStr;
        }
    }
    
}