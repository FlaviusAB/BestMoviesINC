namespace Api.Models;

public class FavoriteEntity
{
    public FavoriteEntity(string username, string movieId)
    {
        Username = username;
        MovieId = movieId;
    }

    public string Username { get; set; }
    public string MovieId { get; set; }
}