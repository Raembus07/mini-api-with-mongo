using MongoDB.Driver;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var movieDatabaseConfigSection = builder.Configuration.GetSection("DatabaseSettings");
builder.Services.Configure<DatabaseSettings>(movieDatabaseConfigSection);

builder.Services.AddSingleton<IMovieService, MongoMovieService>();

var app = builder.Build();

app.MapGet("/", () => "Minimal API Version 1.0");

app.MapGet("/check", (IMovieService movieService) =>
{
    return movieService.Check();
});

// POST – Insert Movie
app.MapPost("/api/movies", (IMovieService service, Movie movie) =>
{
    return service.InsertMovie(movie);
});

// GET - Get all movies
app.MapGet("/api/movies", (IMovieService service) =>
{
    return service.GetAllMovies();
});

// GET{id} - Get movie by id
app.MapGet("/api/movies/{id}", (IMovieService service, string id) =>
{
    return service.GetMovieById(id);
});

//PUT{id} - Update movie by id
app.MapPut("/api/movies/{id}", (IMovieService service, string id, Movie updatedMovie) =>
{
    return service.UpdateMovie(id, updatedMovie);
});

//DELETE{id} - Delete movie by id
app.MapDelete("/api/movies/{id}", (IMovieService service, string id) =>
{
    return service.DeleteMovie(id);
});

// POST - Insert bulk movies
app.MapPost("/api/movies/bulk", (IMovieService service, List<Movie> movies) =>
{
    return service.InsertBulkMovies(movies);
});


app.Run();