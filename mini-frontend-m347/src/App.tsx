import { useEffect, useState } from "react";
import type Movie from "./Movie";
import "./App.css";

function App() {
  const [movies, setMovies] = useState<Movie[]>([]);

  useEffect(() => {
    fetch("http://localhost:5001/api/movies")
      .then(res => res.json())
      .then(data => setMovies(data));
      console.log("Movies fetched:", movies);
  }, []);

  return (
    <div className="container">
      <h1>🎬 Movie Collection</h1>
      <div className="movie-grid">
        {movies.map(movie => (
          console.log("Movie:", movie),
          <div className="movie-card" key={movie.id}>
            <h2>{movie.title} <span className="year">({movie.year})</span></h2>
            <div className="summary">{movie.summary}</div>
            <div className="info-row">
              <strong>Actors:</strong> {movie.actors.join(", ")}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

export default App;