import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { api } from "../Auth/apiClient";
import type { Item, Listing } from "../api/Api";
import "./MainPage.css";
import logo from "../assets/SatinRoadLogo.png";
import { AuthStatus } from "../components/AuthStatus";

import blazeRodImage from "../assets/product-images/Blaze-rod.png";
import wheatImage from "../assets/product-images/Wheat.png";
import sugarImage from "../assets/product-images/Sugar.png";
import sugarCaneImage from "../assets/product-images/Sugar-cane.png";
import tntImage from "../assets/product-images/TNT.png";
import seedsImage from "../assets/product-images/Seeds.png";

const itemImages: Record<number, string> = {
    1: blazeRodImage,
    2: wheatImage,
    3: sugarImage,
    4: sugarCaneImage,
    5: tntImage,
    6: seedsImage,
};

export function MainPage() {
    const [items, setItems] = useState<Item[]>([]);
    const [listings, setListings] = useState<Listing[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        async function loadData() {
            try {
                const loadedItems = await api.api.itemGetAll();
                const loadedListings = await api.api.listingGetAll();

                setItems(loadedItems);
                setListings(loadedListings);
            } catch {
                setError("Could not load items and listings.");
            } finally {
                setLoading(false);
            }
        }

        loadData();
    }, []);

    return (
        <section className="main-page">
            <AuthStatus />

            <header className="main-page-title">
                <h1>
                    <img src={logo} alt="SatinRoad" className="SatinRoadLogo" />
                </h1>
            </header>

            {loading && <p>Loading items...</p>}
            {error && <p>{error}</p>}

            <div className="product-grid">
                {items.map((item) => {
                    const itemListings = listings.filter(
                        (listing) => listing.itemId === item.id,
                    );

                    const cheapest =
                        itemListings.length > 0
                            ? Math.min(
                                ...itemListings.map(
                                    (listing) => listing.price ?? 0,
                                ),
                            )
                            : undefined;

                    const amountListed = itemListings.reduce(
                        (total, listing) =>
                            total + (listing.quantity ?? 0),
                        0,
                    );

                    return (
                        <article key={item.id} className="product-card">
                            <Link
                                to={`/products/${item.id}`}
                                className="product-image"
                            >
                                <img
                                    src={itemImages[item.id ?? 0]}
                                    alt={item.name}
                                />
                            </Link>

                            <div className="product-info">
                                <h2>
                                    <Link
                                        to={`/products/${item.id}`}
                                        className="product-name"
                                    >
                                        {item.name}
                                    </Link>
                                </h2>

                                <p>
                                    {cheapest !== undefined
                                        ? `From ${cheapest.toFixed(2)} kr`
                                        : "No listings"}
                                </p>

                                <p>{amountListed} listed</p>
                            </div>
                        </article>
                    );
                })}
            </div>
        </section>
    );
}