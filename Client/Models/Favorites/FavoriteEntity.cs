namespace Client.Models.Favorites;

public class FavoriteEntity
{
    public FavoriteEntity(string username, string movieId)
    {
        this.Username = username;
        MovieId = movieId;
    }

    public string Username { get; set; }
    public string MovieId { get; set; }
}