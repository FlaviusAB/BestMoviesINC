namespace Api.Models;

public class MovieReviewEntity
{
    public MovieReviewEntity(string username, string movieId, string review)
    {
        this.username = username;
        movie_id = movieId;
        this.review = review;
    }

    public string username { get; set; }
    public string movie_id { get; set; }
    public string review { get; set; }
}