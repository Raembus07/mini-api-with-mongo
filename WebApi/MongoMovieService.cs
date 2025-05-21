using Microsoft.Extensions.Options;
using MongoDB.Driver;
public class MongoMovieService : IMovieService
{
    private readonly IMongoCollection<Movie> _movieCollection;
    private readonly IOptions<DatabaseSettings> options;

    public MongoMovieService(IOptions<DatabaseSettings> options)
    {
        this.options = options;
        var mongoDbConnectionString = options.Value.ConnectionString;
        var mongoClient = new MongoClient(mongoDbConnectionString);
        var databaseName = new MongoUrl(mongoDbConnectionString).DatabaseName;
        var database = mongoClient.GetDatabase(databaseName);
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
}