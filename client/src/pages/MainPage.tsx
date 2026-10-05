import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { api } from "../Auth/apiClient";
import type { Category, Item, Listing } from "../api/Api";
import "./MainPage.css";
import logo from "../assets/SatinRoadLogo.png";
import { AuthStatus } from "../components/AuthStatus";

import blazeRodImage from "../assets/product-images/Blaze-rod.png";
import wheatImage from "../assets/product-images/Wheat.png";
import sugarImage from "../assets/product-images/Sugar.png";
import sugarCaneImage from "../assets/product-images/Sugar-cane.png";
import tntImage from "../assets/product-images/TNT.png";
import seedsImage from "../assets/product-images/Seeds.png";
import ironSwordImage from "../assets/product-images/Iron-sword.png";
import goldHelmetImage from "../assets/product-images/Golden-helmet.png";
import stick from "../assets/product-images/Stick.gif";

const itemImages: Record<number, string> = {
    1: blazeRodImage,
    2: wheatImage,
    3: sugarImage,
    4: sugarCaneImage,
    5: tntImage,
    6: seedsImage,
    7: ironSwordImage,
    8: goldHelmetImage,
    9: stick,
};

export function MainPage() {
    const [items, setItems] = useState<Item[]>([]);
    const [listings, setListings] = useState<Listing[]>([]);
    const [categories, setCategories] = useState<Category[]>([]);
    // "" means "All categories"
    const [selectedCategory, setSelectedCategory] = useState("");
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        async function loadData() {
            try {
                // Load all three at the same time instead of one after another
                const [loadedItems, loadedListings, loadedCategories] = await Promise.all([
                    api.api.itemGetAll(),
                    api.api.listingGetAll(),
                    api.api.categoryGetAll(),
                ]);

                setItems(loadedItems);
                setListings(loadedListings);
                setCategories(loadedCategories);
            } catch {
                setError("Could not load items and listings.");
            } finally {
                setLoading(false);
            }
        }

        loadData();
    }, []);

    const visibleItems = selectedCategory
        ? items.filter((item) => item.categoryId === selectedCategory)
        : items;

    return (
        <section className="main-page">
            <AuthStatus />

            <header className="main-page-title">
                <h1>
                    <img src={logo} alt="SatinRoad" className="main-page-logo" />
                </h1>
            </header>

            {loading && <p>Loading items...</p>}
            {error && <p>{error}</p>}

            <div className="main-page-toolbar">
                <label className="category-filter">
                    <span>Category</span>
                    <select
                        className="category-select"
                        value={selectedCategory}
                        onChange={(event) => setSelectedCategory(event.target.value)}
                    >
                        <option value="">All</option>
                        {categories.map((category) => (
                            <option key={category.id} value={category.id ?? ""}>
                                {category.name}
                            </option>
                        ))}
                    </select>
                </label>
            </div>

            {!loading && visibleItems.length === 0 && (
                <p>No items in this category yet.</p>
            )}

            <div className="product-grid">
                {visibleItems.map((item) => {
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