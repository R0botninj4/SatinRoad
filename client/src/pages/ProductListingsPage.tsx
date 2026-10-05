import { Fragment, useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { api } from "../Auth/apiClient";
import { useAuth } from "../Auth/AuthProvider";
import type { Item, ListingResponseDto as ApiListing } from "../api/Api";
import { BuyDialog } from "../components/BuyDialog";
import { SellDialog, type NewListing } from "../components/SellDialog";
import "./ProductListingsPage.css";

import blazeRodImage from "../assets/product-images/Blaze-rod.png";
import wheatImage from "../assets/product-images/Wheat.png";
import sugarImage from "../assets/product-images/Sugar.png";
import sugarCaneImage from "../assets/product-images/Sugar-cane.png";
import tntImage from "../assets/product-images/TNT.png";
import seedsImage from "../assets/product-images/Seeds.png";
import ironSwordImage from "../assets/product-images/Iron-sword.png";
import goldHelmetImage from "../assets/product-images/Golden-helmet.png";
import stick from "../assets/product-images/Stick.webp";

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

export function ProductListingsPage() {
    const { id } = useParams();
    const { user } = useAuth();
    const itemId = Number(id);

    const [item, setItem] = useState<Item | null>(null);
    const [listings, setListings] = useState<ApiListing[]>([]);
    const [buyingListing, setBuyingListing] = useState<ApiListing | null>(null);
    const [selling, setSelling] = useState(false);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        async function loadData() {
            try {
                const [items, allListings] = await Promise.all([
                    api.api.itemGetAll(),
                    api.api.listingGetAll(),
                ]);

                const selectedItem = items.find(
                    (currentItem) => currentItem.id === itemId,
                );

                setItem(selectedItem ?? null);

                const selectedListings = allListings.filter(
                    (listing) => listing.itemId === itemId,
                );

                setListings(selectedListings);
            } catch {
                setError("Could not load listings.");
            } finally {
                setLoading(false);
            }
        }

        loadData();
    }, [itemId]);

    function openBuyDialog(listing: ApiListing) {
        setBuyingListing(listing);
    }

    async function handleCreateListing(newListing: NewListing) {
        try {
            await api.api.listingCreate({
                itemId,
                price: newListing.pricePerItem,
                quantity: newListing.amount,
                description: "",
            });

            const allListings = await api.api.listingGetAll();

            setListings(
                allListings.filter((listing) => listing.itemId === itemId),
            );

            setSelling(false);
        } catch {
            setError("Could not create listing.");
        }
    }

    if (loading) {
        return (
            <section className="listings-page">
                <p>Loading listings...</p>
            </section>
        );
    }

    if (error) {
        return (
            <section className="listings-page">
                <p>{error}</p>
                <Link to="/">Back to main page</Link>
            </section>
        );
    }

    if (!item) {
        return (
            <section className="listings-page">
                <p>Product not found.</p>
                <Link to="/">Back to main page</Link>
            </section>
        );
    }

    return (
        <section className="listings-page">
            <Link to="/" className="back-link">
                Go back to main page
            </Link>

            <div className="listings-product">
                <div className="listings-product-image">
                    <img
                        src={itemImages[item.id ?? 0]}
                        alt={item.name}
                    />
                </div>

                <h1>{item.name}</h1>
            </div>

            <div className="listings-toolbar">
                <span>
                    {listings.length}{" "}
                    {listings.length === 1 ? "listing" : "listings"}
                </span>

                {user ? (
                    <button
                        type="button"
                        className="stone-button"
                        onClick={() => setSelling(true)}
                    >
                        + Create listing
                    </button>
                ) : (
                    <Link to="/login" className="stone-button">
                        Log in to sell
                    </Link>
                )}
            </div>

            {listings.length === 0 ? (
                <p>No one is selling this product right now.</p>
            ) : (
                <table className="listings-table">
                    <thead>
                    <tr>
                        <th>Vendor</th>
                        <th>Amount</th>
                        <th>Price per item</th>
                        <th>
                                <span className="visually-hidden">
                                    Actions
                                </span>
                        </th>
                    </tr>
                    </thead>

                    <tbody>
                    {listings.map((listing) => (
                        <Fragment key={listing.id}>
                            <tr
                                className="sign-divider"
                                aria-hidden="true"
                            >
                                <td colSpan={4} />
                            </tr>

                            <tr>
                                <td>
                                    {listing.username ?? "Unknown vendor"}
                                    {listing.isFeatured && <span title="Featured vendor"> ★</span>}
                                </td>

                                <td>{listing.quantity ?? 0}</td>

                                <td>
                                    {(listing.price ?? 0).toFixed(2)} kr
                                </td>

                                <td>
                                    {user ? (
                                        <button
                                            type="button"
                                            className="stone-button"
                                            onClick={() =>
                                                openBuyDialog(listing)
                                            }
                                        >
                                            Buy
                                        </button>
                                    ) : (
                                        <Link
                                            to="/login"
                                            className="stone-button"
                                        >
                                            Log in to buy
                                        </Link>
                                    )}
                                </td>
                            </tr>
                        </Fragment>
                    ))}
                    </tbody>
                </table>
            )}

            {buyingListing && (
                <BuyDialog
                    listing={buyingListing}
                    productName={item.name ?? ""}
                    onClose={() => setBuyingListing(null)}
                    onBought={async () => {
                        const allListings = await api.api.listingGetAll();

                        setListings(
                            allListings.filter((listing) => listing.itemId === itemId),
                        );

                        setBuyingListing(null);
                    }}
                />
            )}

            {selling && (
                <SellDialog
                    productName={item.name ?? ""}
                    onSubmit={handleCreateListing}
                    onClose={() => setSelling(false)}
                />
            )}
        </section>
    );
}
