namespace Api.Models;

public class MovieReviewEntity
{
    public MovieReviewEntity(string username, string movieId, string review)
    {
        this.Username = username;
        MovieId = movieId;
        this.Review = review;
    }

    public string Username { get; set; }
    public string MovieId { get; set; }
    public string Review { get; set; }
}