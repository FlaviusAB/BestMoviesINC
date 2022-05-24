using System;
using System.IO;
using System.Threading.Tasks;
using Api.Models;
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
           
            try
            {
                var appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");
                using (SqlConnection conn = new SqlConnection(appsettingvalue))  
                {
                    conn.Open();  
                    if(!String.IsNullOrEmpty(input.username))  
                    {  
                        Console.WriteLine(input.username);
                        
                        var query = $"select username from login where username='{input.username}' and password='{input.password}'";  
                        SqlCommand command = new SqlCommand(query, conn);
                        
                        var reader = await command.ExecuteReaderAsync();
                        while (reader.Read())
                        {

                            foundUsername = reader["username"].ToString();

                        }

                        if (!string.IsNullOrWhiteSpace(foundUsername))
                        {
                            authenticated = true;
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
                return await Task.FromResult(new OkObjectResult(token)).ConfigureAwait(false);
            }
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
                    if(!String.IsNullOrEmpty(input.username))  
                    {  
                        Console.WriteLine(input.email +" EMAIL");
                        Console.WriteLine(input.username +" USERNAME");
                        var query = $"INSERT INTO [login] (username,password,email,accessType) VALUES('{input.username}', '{input.password}' , '{input.email}' , '{input.accessType}')";  
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