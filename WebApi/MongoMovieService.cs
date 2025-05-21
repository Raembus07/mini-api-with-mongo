using Microsoft.Extensions.Options;
using MongoDB.Driver;
public class MongoMovieService : IMovieService
{
    private readonly IMongoCollection<Movie> _movieCollection;
    private readonly IOptions<DatabaseSettings> options;


    public MongoMovieService(IOptions<DatabaseSettings> options)
    {
        this.options = options;
        var client = new MongoClient(options.Value.ConnectionString);
        var database = client.GetDatabase("movies"); // Passe ggf. den DB-Namen an
        _movieCollection = database.GetCollection<Movie>("movies");
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
        var movies = _movieCollection.Find(_ => true).ToList();
        return Results.Ok(movies);
    }

    public IResult Create(Movie movie)
    {
        if (_movieCollection.Find(m => m.Id == movie.Id).Any())
        {
            return Results.Conflict($"Movie with id {movie.Id} already exists.");
        }
        _movieCollection.InsertOne(movie);
        return Results.Ok(movie);
    }

    public IResult Get(string id)
    {
        var movie = _movieCollection.Find(m => m.Id == id).FirstOrDefault();
        return movie is not null ? Results.Ok(movie) : Results.NotFound();
    }

    public IResult Update(string id, Movie updatedMovie)
    {
        var result = _movieCollection.ReplaceOne(m => m.Id == id, updatedMovie);
        if (result.MatchedCount == 0)
            return Results.NotFound();
        return Results.Ok(updatedMovie);
    }

    public IResult Delete(string id)
    {
        var result = _movieCollection.DeleteOne(m => m.Id == id);
        if (result.DeletedCount == 0)
            return Results.NotFound();
        return Results.Ok();
    }

    public IResult Create(List<Movie> movies)
    {
        var existingIds = _movieCollection.Find(m => movies.Select(x => x.Id).Contains(m.Id)).ToList();
        if (existingIds.Any())
        {
            return Results.Conflict($"Movie with id {string.Join(", ", existingIds.Select(m => m.Id))} already exists.");
        }
        _movieCollection.InsertMany(movies);
        return Results.Ok(movies);
    }

}