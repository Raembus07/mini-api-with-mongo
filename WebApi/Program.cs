using MongoDB.Driver;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var movieDatabaseConfigSection = builder.Configuration.GetSection("DatabaseSettings");
builder.Services.Configure<DatabaseSettings>(movieDatabaseConfigSection);

var app = builder.Build();
var movies = new List<Movie>();

app.MapGet("/", () => "Minimal API Version 1.0");

app.MapGet("/check", (IOptions<DatabaseSettings> options) =>
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
});

// POST – Insert Movie
app.MapPost("/api/movies", (Movie movie) =>
{
    if (movies.Any(m => m.Id == movie.Id))
    {
        return Results.Conflict($"Movie with id {movie.Id} already exists.");
    }

    movies.Add(movie);
    return Results.Ok(movie);
});

app.MapPost("/api/movies", (Movie movie) =>
{
    if (movies.Any(m => m.Id == movie.Id))
    {
        return Results.Conflict($"Movie with id {movie.Id} already exists.");
    }

    movies.Add(movie);
    return Results.Ok(movie);
});

app.MapGet("/api/movies", () =>
{
    return Results.Ok(movies);
});

app.MapGet("/api/movies/{id}", (string id) =>
{
    var movie = movies.FirstOrDefault(m => m.Id == id);
    return movie is not null ? Results.Ok(movie) : Results.NotFound();
});

app.MapPut("/api/movies/{id}", (string id, Movie updatedMovie) =>
{
    var index = movies.FindIndex(m => m.Id == id);
    if (index == -1)
        return Results.NotFound();

    movies[index] = updatedMovie;
    return Results.Ok(updatedMovie);
});

app.MapDelete("/api/movies/{id}", (string id) =>
{
    var movie = movies.FirstOrDefault(m => m.Id == id);
    if (movie is null)
        return Results.NotFound();

    movies.Remove(movie);
    return Results.Ok();
});


app.Run();