using MongoDB.Driver;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var movieDatabaseConfigSection = builder.Configuration.GetSection("DatabaseSettings");
builder.Services.Configure<DatabaseSettings>(movieDatabaseConfigSection);

builder.Services.AddSingleton<IMovieService, MongoMovieService>();

var app = builder.Build();
var movies = new List<Movie>();

app.MapGet("/", () => "Minimal API Version 1.0");

app.MapGet("/check", (IMovieService movieService) =>
{
    return movieService.Check();
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

// GET - Get all movies
app.MapGet("/api/movies", () =>
{
    return Results.Ok(movies);
});

// GET{id} - Get movie by id
app.MapGet("/api/movies/{id}", (string id) =>
{
    var movie = movies.FirstOrDefault(m => m.Id == id);
    return movie is not null ? Results.Ok(movie) : Results.NotFound();
});

//PUT{id} - Update movie by id
app.MapPut("/api/movies/{id}", (string id, Movie updatedMovie) =>
{
    var index = movies.FindIndex(m => m.Id == id);
    if (index == -1)
        return Results.NotFound();

    movies[index] = updatedMovie;
    return Results.Ok(updatedMovie);
});

//DELETE{id} - Delete movie by id
app.MapDelete("/api/movies/{id}", (string id) =>
{
    var movie = movies.FirstOrDefault(m => m.Id == id);
    if (movie is null)
        return Results.NotFound();

    movies.Remove(movie);
    return Results.Ok();
});

app.MapPost("/api/movies/bulk", (List<Movie> movies) =>
{
    foreach (var movie in movies)
    {
        if (movies.Any(m => m.Id == movie.Id))
        {
            return Results.Conflict($"Movie with id {movie.Id} already exists.");
        }
    }

    movies.AddRange(movies);
    return Results.Ok(movies);
});


app.Run();