namespace Client.Models;

public class FavoriteEntity
{
    public FavoriteEntity(string username, string movieId, bool favorite)
    {
        this.username = username;
        movie_id = movieId;
        this.favorite = favorite;
    }

    public string username { get; set; }
    public string movie_id { get; set; }
    public bool favorite { get; set; }
}