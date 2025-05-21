public interface IMovieService
{
    IResult Check();

    IResult Get();
    IResult Get(string id);
    IResult Create(Movie movie);
    IResult Update(string id, Movie updatedMovie);
    IResult Delete(string id);
    IResult Create(List<Movie> movies);
}