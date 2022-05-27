namespace Client.Models;

public class FavoriteEntity
{
    public FavoriteEntity(string username, string movieId)
    {
        this.username = username;
        movie_id = movieId;
    }

    public string username { get; set; }
    public string movie_id { get; set; }

}