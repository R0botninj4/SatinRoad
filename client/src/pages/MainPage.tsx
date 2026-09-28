import "./MainPage.css";

// Temporary fake data – will be replaced with data from the backend
type Product = {
    id: number;
    name: string;
    cheapestPrice: number;
    amountListed: number;
};

const products: Product[] = [
    { id: 1, name: "Blaze rod", cheapestPrice: 12.5, amountListed: 34 },
    { id: 2, name: "Wheat", cheapestPrice: 45, amountListed: 8 },
    { id: 3, name: "Sugar", cheapestPrice: 120, amountListed: 3 },
    { id: 4, name: "Sugar cane", cheapestPrice: 10, amountListed: 50 },
    { id: 5, name: "TNT", cheapestPrice: 25, amountListed: 6 },
    { id: 6, name: "Seeds", cheapestPrice: 5, amountListed: 100 },
];

export function MainPage() {
    return (
        <section className="main-page">
            <header className="main-page-title">
                <h1>Minecraft Road</h1>
            </header>

            <div className="product-grid">
                {products.map((product) => (
                    <article key={product.id} className="product-card">
                        <div className="product-image">Product image</div>

                        <div className="product-info">
                            <h2>{product.name}</h2>
                            <p>From {product.cheapestPrice.toFixed(2)} kr</p>
                            <p>{product.amountListed} listed</p>
                        </div>
                    </article>
                ))}
            </div>
        </section>
    );
}