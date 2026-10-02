import { Fragment, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { getListingsForProduct, products, type Listing } from "../TempData/Mockdata.ts";
import { BuyDialog } from "../components/BuyDialog";
import "./ProductListingsPage.css";

export function ProductListingsPage() {
    const { id } = useParams();
    const product = products.find((p) => p.id === Number(id));
    // The listing the user clicked "Buy" on (null = pop-up closed)
    const [buyingListing, setBuyingListing] = useState<Listing | null>(null);

    if (!product) {
        return (
            <section className="listings-page">
                <p>Product not found.</p>
                <Link to="/">Back to main page</Link>
            </section>
        );
    }

    const productListings = getListingsForProduct(product.id);

    return (
        <section className="listings-page">
            <Link to="/" className="back-link">Go back to main page</Link>

            <div className="listings-product">
                <div className="listings-product-image">
                    <img src={product.image} alt="" />
                </div>
                <h1>{product.name}</h1>
            </div>

            {productListings.length === 0 ? (
                <p>No one is selling this product right now.</p>
            ) : (
                <table className="listings-table">
                    <thead>
                    <tr>
                        <th>Vendor</th>
                        <th>Amount</th>
                        <th>Price per item</th>
                        <th><span className="visually-hidden">Actions</span></th>
                    </tr>
                    </thead>
                    <tbody>
                    {productListings.map((listing) => (
                        <Fragment key={listing.id}>
                            <tr className="sign-divider" aria-hidden="true">
                                <td colSpan={4} />
                            </tr>
                            <tr>
                                <td>{listing.vendor}</td>
                                <td>{listing.quantity}</td>
                                <td>{listing.pricePerItem.toFixed(2)} kr</td>
                                <td>
                                    <button
                                        type="button"
                                        className="stone-button"
                                        onClick={() => setBuyingListing(listing)}
                                        aria-label={`Buy ${product.name} from ${listing.vendor}`}
                                    >
                                        Buy
                                    </button>
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
                    productName={product.name}
                    onClose={() => setBuyingListing(null)}
                />
            )}
        </section>
    );
}