// Temporary fake data – will be replaced with data from the backend
import blazeRodImage from "../assets/product-images/Blaze-rod.png";
import wheatImage from "../assets/product-images/Wheat.png";
import sugarImage from "../assets/product-images/Sugar.png";
import sugarCaneImage from "../assets/product-images/Sugar-cane.png";
import tntImage from "../assets/product-images/Tnt.png";
import seedsImage from "../assets/product-images/Seeds.png";

export type Product = {
    id: number;
    name: string;
    image: string;
};

export type Listing = {
    id: number;
    productId: number;
    vendor: string;
    quantity: number;
    pricePerItem: number;
};

export const products: Product[] = [
    { id: 1, name: "Blaze rod", image: blazeRodImage },
    { id: 2, name: "Wheat", image: wheatImage },
    { id: 3, name: "Sugar", image: sugarImage },
    { id: 4, name: "Sugar cane", image: sugarCaneImage },
    { id: 5, name: "TNT", image: tntImage },
    { id: 6, name: "Seeds", image: seedsImage },
];

export const listings: Listing[] = [
    { id: 1, productId: 1, vendor: "Steve", quantity: 20, pricePerItem: 12.5 },
    { id: 2, productId: 1, vendor: "Alex", quantity: 14, pricePerItem: 15 },
    { id: 3, productId: 2, vendor: "Steve", quantity: 8, pricePerItem: 45 },
    { id: 4, productId: 3, vendor: "Notch", quantity: 3, pricePerItem: 120 },
    { id: 5, productId: 4, vendor: "Alex", quantity: 50, pricePerItem: 10 },
    { id: 6, productId: 5, vendor: "Herobrine", quantity: 6, pricePerItem: 25 },
    { id: 7, productId: 6, vendor: "Steve", quantity: 100, pricePerItem: 10 },
    { id: 8, productId: 6, vendor: "Alex", quantity: 64, pricePerItem: 5 },
    { id: 9, productId: 6, vendor: "Notch", quantity: 20, pricePerItem: 8 },
];

// All listings for one product, cheapest first
export function getListingsForProduct(productId: number): Listing[] {
    return listings
        .filter((listing) => listing.productId === productId)
        .sort((a, b) => a.pricePerItem - b.pricePerItem);
}