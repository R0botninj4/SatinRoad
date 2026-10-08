import { Fragment, useEffect, useState } from "react";
import { Link, Navigate } from "react-router-dom";
import { api } from "../Auth/apiClient";
import { useAuth } from "../Auth/AuthProvider";
import type { Item, Listing, Order } from "../api/Api";
import "./ProfilePage.css";

export function ProfilePage() {
    const { user, loading: authLoading } = useAuth();

    const [items, setItems] = useState<Item[]>([]);
    const [listings, setListings] = useState<Listing[]>([]);
    const [orders, setOrders] = useState<Order[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [deleteError, setDeleteError] = useState("");

    useEffect(() => {
        if (authLoading || !user || user.isShutDown) return;

        async function loadData() {
            try {
                const [loadedItems, allListings, myOrders] = await Promise.all([
                    api.api.itemGetAll(),
                    api.api.listingGetAll(),
                    api.api.orderGetMyOrders(),
                ]);

                setItems(loadedItems);
                setListings(allListings);
                setOrders(myOrders);
            } catch {
                setError("Could not load your profile.");
            } finally {
                setLoading(false);
            }
        }

        loadData();
    }, [authLoading, user?.id, user?.isShutDown]);

    // Not logged in: send to the login page.
    if (!authLoading && !user) return <Navigate to="/login" replace />;
    if (authLoading || !user) return null;

    const myListings = listings.filter((listing) => listing.userId === user.id);

    function itemName(itemId?: number) {
        return items.find((item) => item.id === itemId)?.name ?? "Unknown item";
    }

    // Orders remember their product. Older orders don't, so fall back to the listing.
    function orderItemId(order: Order) {
        return order.itemId || listings.find((listing) => listing.id === order.listingId)?.itemId;
    }

    // Only the 5 newest purchases are shown.
    const latestOrders = [...orders]
        .sort((first, second) => (second.createdAt ?? "").localeCompare(first.createdAt ?? ""))
        .slice(0, 5);

    async function handleDelete(listing: Listing) {
        if (!listing.id) return;
        if (!window.confirm("Delete this listing?")) return;

        try {
            await api.api.listingDelete({ id: listing.id });
            setListings((current) => current.filter((currentListing) => currentListing.id !== listing.id));
            setDeleteError("");
        } catch {
            setDeleteError("Could not delete the listing.");
        }
    }

    return (
        <section className="profile-page">
            <Link to="/" className="profile-back">Go back to main page</Link>

            <h1 className="profile-name">{user.username}</h1>

            {user.isShutDown ? (
                <div className="profile-busted" role="status">
                    <h2>Disconnected</h2>
                    <p>The FBI has permanently shut down your shop.</p>
                    <p>All your listings have been removed. You can still buy, but you cannot sell again.</p>
                </div>
            ) : (
                <>
                    {loading && <p>Loading profile...</p>}
                    {error && <p>{error}</p>}
                </>
            )}

            {!user.isShutDown && !loading && !error && (
                <div className="profile-lists">
                    <div className="profile-sign">
                        <h2 className="profile-sign-title">Your listings</h2>
                        {deleteError && <p>{deleteError}</p>}

                        {myListings.length === 0 ? (
                            <p>You are not selling anything right now.</p>
                        ) : (
                            <table className="profile-table">
                                <thead>
                                <tr>
                                    <th>Product</th>
                                    <th>Amount</th>
                                    <th>Price per item</th>
                                    <th>
                                        <span className="visually-hidden">Actions</span>
                                    </th>
                                </tr>
                                </thead>

                                <tbody>
                                {myListings.map((listing) => (
                                    <Fragment key={listing.id}>
                                        <tr className="sign-divider" aria-hidden="true">
                                            <td colSpan={4} />
                                        </tr>

                                        <tr>
                                            <td>
                                                <Link to={`/products/${listing.itemId}`} className="profile-link">
                                                    {itemName(listing.itemId)}
                                                </Link>
                                            </td>
                                            <td>{listing.quantity ?? 0}</td>
                                            <td>{(listing.price ?? 0).toFixed(2)} kr</td>
                                            <td>
                                                <button
                                                    type="button"
                                                    className="stone-button profile-delete"
                                                    onClick={() => handleDelete(listing)}
                                                >
                                                    Delete
                                                </button>
                                            </td>
                                        </tr>
                                    </Fragment>
                                ))}
                                </tbody>
                            </table>
                        )}
                    </div>

                    <div className="profile-sign">
                        <h2 className="profile-sign-title">Your purchases</h2>

                        {latestOrders.length === 0 ? (
                            <p>You have not bought anything yet.</p>
                        ) : (
                            <table className="profile-table">
                                <thead>
                                <tr>
                                    <th>Product</th>
                                    <th>Amount</th>
                                    <th>Total price</th>
                                </tr>
                                </thead>

                                <tbody>
                                {latestOrders.map((order) => (
                                    <Fragment key={order.id}>
                                        <tr className="sign-divider" aria-hidden="true">
                                            <td colSpan={3} />
                                        </tr>

                                        <tr>
                                            <td>
                                                <Link to={`/products/${orderItemId(order)}`} className="profile-link">
                                                    {itemName(orderItemId(order))}
                                                </Link>
                                            </td>
                                            <td>{order.quantity ?? 0}</td>
                                            <td>{(order.totalPrice ?? 0).toFixed(2)} kr</td>
                                        </tr>
                                    </Fragment>
                                ))}
                                </tbody>
                            </table>
                        )}
                    </div>
                </div>
            )}
        </section>
    );
}
