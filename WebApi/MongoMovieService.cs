using Microsoft.Extensions.Options;
using MongoDB.Driver;
public class MongoMovieService : IMovieService
{
    private readonly IMongoCollection<Movie> _movieCollection;
    private readonly IOptions<DatabaseSettings> options;
    List<Movie> movies = new List<Movie>();


    public MongoMovieService(IOptions<DatabaseSettings> options)
    {
        this.options = options;
    }
    public IResult Check()
    {
        var mongoDbConnectionString = options.Value.ConnectionString;

        var error = "Fehler beim Zugriff auf MongoDB";
        try
        {
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var cancellationToken = cancellationTokenSource.Token;

            var client = new MongoClient(mongoDbConnectionString);

            var databases = client.ListDatabaseNames(cancellationToken).ToList();

            return Results.Ok("Zugriff auf MongoDB ok. Datenbanken: " + string.Join(", ", databases));
        }
        catch (TimeoutException ex)
        {
            return Results.Problem(error + " (timout): " + ex.Message);
        }
        catch (Exception ex)
        {
            return Results.Problem(error + " :" + ex.Message);
        }
    }

    public IResult Get()
    {
        return Results.Ok(movies);
    }

    public IResult Create(Movie movie)
    {
        if (movies.Any(m => m.Id == movie.Id))
        {
            return Results.Conflict($"Movie with id {movie.Id} already exists.");
        }

        movies.Add(movie);
        return Results.Ok(movie);
    }

    public IResult Get(string id)
    {
        var movie = movies.FirstOrDefault(m => m.Id == id);
        return movie is not null ? Results.Ok(movie) : Results.NotFound();
    }

    public IResult Update(string id, Movie updatedMovie)
    {
        var index = movies.FindIndex(m => m.Id == id);
        if (index == -1)
            return Results.NotFound();

        movies[index] = updatedMovie;
        return Results.Ok(updatedMovie);
    }

    public IResult Delete(string id)
    {
        var movie = movies.FirstOrDefault(m => m.Id == id);
        if (movie is null)
            return Results.NotFound();

        movies.Remove(movie);
        return Results.Ok();
    }

    public IResult Create(List<Movie> movies)
    {
        foreach (var movie in movies)
        {
            if (this.movies.Any(m => m.Id == movie.Id))
            {
                return Results.Conflict($"Movie with id {movie.Id} already exists.");
            }
            this.movies.Add(movie);
        }
        return Results.Ok(movies);
    }

}