import { useState } from "react";
import { Api, type WeatherForecast } from "./api/Api";
import "./index.css";

const api = new Api({
    baseUrl: "http://localhost:5188",
});

export function App() {
    const [forecasts, setForecasts] = useState<WeatherForecast[]>([]);
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);

    async function testConnection() {
        setLoading(true);
        setError("");
        setForecasts([]);

        try {
            const result = await api.weatherForecast.weatherForecastGet();
            setForecasts(result);
        } catch {
            setError("Kunne ikke hente data fra backend.");
        } finally {
            setLoading(false);
        }
    }

    return (
        <div className="app">
            <h1>SatinRoad – forbindelsestest</h1>

            <button onClick={testConnection} disabled={loading}>
                {loading ? "Henter..." : "Test forbindelse"}
            </button>

            {error && <p role="alert">{error}</p>}

            {forecasts.map((forecast, index) => (
                <p key={forecast.date ?? index}>
                    {forecast.date}: {forecast.temperatureC} °C
                    {" – "}
                    {forecast.summary}
                </p>
            ))}
        </div>
    );
}

export default App;