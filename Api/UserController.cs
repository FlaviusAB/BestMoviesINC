using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Api;

public static class UserController
{
    [FunctionName(nameof(UserAuthenication))]
    public static async Task<IActionResult> UserAuthenication(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth")]
        HttpRequest req, ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request");

        var authenticated = false;

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var input = JsonConvert.DeserializeObject<UserCredentials>(requestBody);
        var authUser = new User();
        var appsettingvalue = "";
        try
        {
            appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");
            using (var conn = new SqlConnection(appsettingvalue))
            {
                conn.Open();
                if (!string.IsNullOrEmpty(input.Username))
                {
                    var query =
                        $"select * from login where username='{input.Username}' and password='{input.Password}'";
                    var command = new SqlCommand(query, conn);

                    var reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        var foundUsername = reader["username"].ToString();
                        if (!string.IsNullOrWhiteSpace(foundUsername))
                        {
                            authenticated = true;
                            authUser.Username = foundUsername;
                            authUser.Email = reader["email"].ToString();
                            authUser.RegistrationDate = reader["registration_date"].ToString();
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


        if (!authenticated) return await Task.FromResult(new UnauthorizedResult()).ConfigureAwait(false);

        GenerateJwtToken generateJwtToken = new();

        var id = Guid.NewGuid();
        var sessionToken = id.ToString();

        using (var conn = new SqlConnection(appsettingvalue))
        {
            conn.Open();
            var query = $"UPDATE login SET sessionToken = '{sessionToken}' WHERE username = '{input.Username}'";
            var command = new SqlCommand(query, conn);

            await command.ExecuteReaderAsync();
        }

        var token = generateJwtToken.IssuingJwt(input.Username, sessionToken);
        authUser.SessionToken = token;
        return await Task.FromResult(new OkObjectResult(authUser)).ConfigureAwait(false);
    }

    [FunctionName("AddReview")]
    public static async Task<IActionResult> AddReview(ExecutionContext context,
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "review")]
        HttpRequest req, ILogger log)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var input = JsonConvert.DeserializeObject<MovieReviewEntity>(requestBody);
        try
        {
            var appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");
            using (var conn = new SqlConnection(appsettingvalue))
            {
                conn.Open();
                if (!string.IsNullOrEmpty(input.Username))
                {
                    var query =
                        $"INSERT INTO [reviews] (username,movie_id,review) VALUES('{input.Username}', '{input.MovieId}', '{input.Review}')";
                    var command = new SqlCommand(query, conn);
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

    [FunctionName("GetReview")]
    public static async Task<IActionResult> GetReview(ExecutionContext context,
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "review/{movie_id}")]
        HttpRequest req, ILogger log, int movie_id)
    {
        var allReviews = new List<MovieReviewEntity>();
        MovieReviewEntity foundReview;

        var appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");

        using (var conn = new SqlConnection(appsettingvalue))
        {
            conn.Open();
            var query = @"select * from reviews where movie_id = @movie_id";
            var command = new SqlCommand(query, conn);

            command.Parameters.AddWithValue("@movie_id", movie_id);
            var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                var usernameL = reader["username"].ToString();
                var movie_idL = reader["movie_id"].ToString();
                var reviewL = reader["review"].ToString();
                foundReview = new MovieReviewEntity(usernameL, movie_idL, reviewL);
                allReviews.Add(foundReview);
            }
        }

        return new OkObjectResult(allReviews);
    }

    [FunctionName("AddFavorite")]
    public static async Task<IActionResult> AddFavorite(ExecutionContext context,
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "favorites")]
        HttpRequest req, ILogger log)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var input = JsonConvert.DeserializeObject<FavoriteEntity>(requestBody);
        try
        {
            var appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");
            using (var conn = new SqlConnection(appsettingvalue))
            {
                conn.Open();
                if (!string.IsNullOrEmpty(input.Username))
                {
                    var query =
                        $"INSERT INTO [favorites] (username,movie_id) VALUES('{input.Username}', '{input.MovieId}')";
                    var command = new SqlCommand(query, conn);
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
        var foundFavorited = "";
        string exists;

        var appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");

        using (var conn = new SqlConnection(appsettingvalue))
        {
            conn.Open();
            var query = @"select * from favorites where username = @username and movie_id = @movie_id";
            var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@movie_id", movie_id);
            var reader = await command.ExecuteReaderAsync();
            while (reader.Read()) foundFavorited = reader["username"].ToString();
            if (string.IsNullOrWhiteSpace(foundFavorited))
                exists = "false";
            else
                exists = "true";
        }

        return new OkObjectResult(exists);
    }

    [FunctionName("GetAllFavorites")]
    public static async Task<IActionResult> GetAllFavorites(ExecutionContext context,
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "favorites/{username}")]
        HttpRequest req, ILogger log, string username)
    {
        var foundMovies = new List<string>();

        var appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");

        using (var conn = new SqlConnection(appsettingvalue))
        {
            conn.Open();
            var query = @"select movie_id from favorites where username = @username";
            var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@username", username);
            var reader = await command.ExecuteReaderAsync();
            while (reader.Read()) foundMovies.Add(reader["movie_id"].ToString());
        }

        return new OkObjectResult(foundMovies);
    }

    [FunctionName("GetFavoriteCount")]
    public static async Task<IActionResult> GetFavoriteCount(ExecutionContext context,
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "favoriteCount/{movie_id}")]
        HttpRequest req, ILogger log, int movie_id)
    {
        var count = "";

        var appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");

        using (var conn = new SqlConnection(appsettingvalue))
        {
            conn.Open();
            var query = @"select count(username) from favorites where movie_id = @movie_id";
            var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@movie_id", movie_id);
            var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                count = reader[""].ToString();
            }
        }

        return new OkObjectResult(count);
    }

    [FunctionName("GetAllUsersFavorite")]
    public static async Task<IActionResult> GetAllUsersFavorite(ExecutionContext context,
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "allFavorite/{var}")]
        HttpRequest req, ILogger log)
    {
        var mostFavorited = new List<string>();

        var appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");

        using (var conn = new SqlConnection(appsettingvalue))
        {
            conn.Open();
            var query =
                @"select top 20 movie_id, count(movie_id) as c from favorites group by movie_id order by c DESC";
            var command = new SqlCommand(query, conn);
            var reader = await command.ExecuteReaderAsync();
            while (reader.Read()) mostFavorited.Add(reader["movie_id"].ToString());
        }

        return new OkObjectResult(mostFavorited);
    }

    [FunctionName("DeleteFavorites")]
    public static async Task<IActionResult> DeleteFavorites(ExecutionContext context,
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "favorites/{username}/{movie_id}")]
        HttpRequest req, ILogger log, string username, int movie_id)
    {
        var foundFavorited = "";
        string exists;

        var appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");

        using (var conn = new SqlConnection(appsettingvalue))
        {
            conn.Open();
            var query = @"delete from favorites where username = @username and movie_id = @movie_id";
            var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@movie_id", movie_id);
            var reader = await command.ExecuteReaderAsync();
            while (reader.Read()) foundFavorited = reader["username"].ToString();
            if (string.IsNullOrWhiteSpace(foundFavorited))
                exists = "false";
            else
                exists = "true";
        }

        return new OkObjectResult(exists);
    }

    [FunctionName("CreateUser")]
    public static async Task<IActionResult> CreateUser(ExecutionContext context,
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "signup")]
        HttpRequest req, ILogger log)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var input = JsonConvert.DeserializeObject<User>(requestBody);
        try
        {
            var appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");
            await using (var conn = new SqlConnection(appsettingvalue))
            {
                conn.Open();
                if (!string.IsNullOrEmpty(input.Username))
                {
                    var query =
                        $"INSERT INTO [login] (username,password,email,registration_date) VALUES('{input.Username}', '{input.Password}' , '{input.Email}','{input.RegistrationDate}')";
                    var command = new SqlCommand(query, conn);
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
        var foundUsername = "";
        string exists;

        var appsettingvalue = GetSqlAzureConnectionString("SQLConnectionString");

        await using (var conn = new SqlConnection(appsettingvalue))
        {
            conn.Open();
            var query = @"select * from login where username = @username";
            var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@username", username);
            var reader = await command.ExecuteReaderAsync();
            while (reader.Read()) foundUsername = reader["username"].ToString();

            exists = string.IsNullOrWhiteSpace(foundUsername) ? "false" : "true";
        }

        return new OkObjectResult(exists);
    }

    [FunctionName(nameof(GetUserData))]
    public static async Task<IActionResult> GetUserData(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "data")]
        HttpRequest req,
        ILogger log)
    {
        // Check if we have authentication info.
        var auth = new ValidateJwt(req);

        if (!auth.IsValid) return new UnauthorizedResult(); // No authentication info.

        var postData = await req.ReadAsStringAsync();

        return new OkObjectResult($"{postData}");
    }

    private static string GetSqlAzureConnectionString(string name)
    {
        var conStr = Environment.GetEnvironmentVariable($"ConnectionStrings:{name}", EnvironmentVariableTarget.Process);
        if (string.IsNullOrEmpty(conStr)) // Azure Functions App Service naming convention
            conStr = Environment.GetEnvironmentVariable($"{name}", EnvironmentVariableTarget.Process);
        return conStr;
    }
}