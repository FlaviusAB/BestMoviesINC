namespace Client.Models;

public class FavoriteEntity
{
    public FavoriteEntity(string username, string movieId, string favorite)
    {
        this.username = username;
        movie_id = movieId;
        this.favorite = favorite;
    }

    public string username { get; set; }
    public string movie_id { get; set; }
    public string favorite { get; set; }
}