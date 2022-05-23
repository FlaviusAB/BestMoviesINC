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
                        Console.WriteLine(input.username +"djksvnskjdb");
                        var query = $"INSERT INTO [login] (username,password,accessType) VALUES('{input.username}', '{input.password}' , '{input.accessType}')";  
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
           
            User user = new User();
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
        public static string GetSqlAzureConnectionString(string name)
        {
            string conStr = System.Environment.GetEnvironmentVariable($"ConnectionStrings:{name}", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(conStr)) // Azure Functions App Service naming convention
                conStr = System.Environment.GetEnvironmentVariable($"{name}", EnvironmentVariableTarget.Process);
            return conStr;
        }
    }
    
}