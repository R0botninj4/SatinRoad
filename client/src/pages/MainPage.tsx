import { Link } from "react-router-dom";
import { getListingsForProduct, products } from "../TempData/Mockdata.ts";
import "./MainPage.css";

export function MainPage() {
    return (
        <section className="main-page">
            <header className="main-page-title">
                <h1>Minecraft Road</h1>
            </header>

            <div className="product-grid">
                {products.map((product) => {
                    const productListings = getListingsForProduct(product.id);
                    const cheapest = productListings[0]?.pricePerItem;
                    const amountListed = productListings.reduce(
                        (sum, listing) => sum + listing.quantity,
                        0,
                    );

                    return (
                        <article key={product.id} className="product-card">
                            <Link to={`/products/${product.id}`} className="product-image">
                                Product image
                            </Link>

                            <div className="product-info">
                                <h2>
                                    <Link to={`/products/${product.id}`} className="product-name">
                                        {product.name}
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