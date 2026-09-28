import { Link, useParams } from "react-router-dom";
import { getListingsForProduct, products } from "../TempData/Mockdata.ts";
import "./ProductListingsPage.css";

export function ProductListingsPage() {
    const { id } = useParams();
    const product = products.find((p) => p.id === Number(id));

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
                <div className="listings-product-image">Product image</div>
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
                    </tr>
                    </thead>
                    <tbody>
                    {productListings.map((listing) => (
                        <tr key={listing.id}>
                            <td>{listing.vendor}</td>
                            <td>{listing.quantity}</td>
                            <td>{listing.pricePerItem.toFixed(2)} kr</td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            )}
        </section>
    );
}