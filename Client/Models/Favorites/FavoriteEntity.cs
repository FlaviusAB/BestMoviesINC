namespace Client.Models.Favorites;

public class FavoriteEntity
{
    public FavoriteEntity(string username, string movieId)
    {
        this.Username = username;
        MovieId = movieId;
    }

    private string Username { get; set; }
    private string MovieId { get; set; }
}