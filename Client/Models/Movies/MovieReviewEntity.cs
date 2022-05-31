namespace Client.Models.Movies;

public class MovieReviewEntity
{
    public MovieReviewEntity(string username, string movieId, string review)
    {
        Username = username;
        MovieId = movieId;
        Review = review;
    }

    public string Username { get; set; }
    private string MovieId { get; set; }
    public string Review { get; set; }
}