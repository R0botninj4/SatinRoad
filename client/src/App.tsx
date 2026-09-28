import { useEffect, useState } from "react";
import { Api, type Category } from "./api/Api";
import "./index.css";

const api = new Api({
    baseUrl: "http://localhost:5188",
});

export function App() {
    const [categories, setCategories] = useState<Category[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    async function loadCategories() {
        setLoading(true);
        setError("");
        setCategories([]);

        try {
            const result = await api.api.categoryGetAll();
            setCategories(result);
        } catch {
            setError("Kunne ikke hente kategorier.");
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        void loadCategories();
    }, []);

    return (
        <div className="app">
            <h1>SatinRoad</h1>
            <h2>Kategorier</h2>

            {loading && <p>Henter kategorier...</p>}

            {error && (
                <div>
                    <p role="alert">{error}</p>
                    <button onClick={loadCategories}>Prøv igen</button>
                </div>
            )}

            {!loading && !error && categories.length === 0 && (
                <p>Der er endnu ingen kategorier.</p>
            )}

            {categories.map(category => (
                <p key={category.id}>{category.name}</p>
            ))}
        </div>
    );
}

export default App;