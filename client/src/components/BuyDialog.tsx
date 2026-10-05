import { useEffect, useRef, useState } from "react";
import type { Listing } from "../api/Api";
import { api } from "../Auth/apiClient";
import { AmountPicker } from "./AmountPicker";
import "./Dialog.css";

type BuyDialogProps = {
    listing: Listing;
    productName: string;
    onClose: () => void;
    onBought: () => void;
};

export function BuyDialog({
                              listing,
                              productName,
                              onClose,
                              onBought,
                          }: BuyDialogProps) {
    const dialogRef = useRef<HTMLDialogElement>(null);
    const [amount, setAmount] = useState(1);
    const [error, setError] = useState("");

    useEffect(() => {
        dialogRef.current?.showModal();
    }, []);

    const price = listing.price ?? 0;
    const quantity = listing.quantity ?? 0;
    const total = amount * price;

    async function handleBuy() {
        if (!listing.id) {
            setError("Listing ID is missing.");
            return;
        }

        try {
            await api.api.orderCreate({
                listingId: listing.id,
                quantity: amount,
            });

            dialogRef.current?.close();
            onBought();
        } catch {
            setError("Could not complete the purchase.");
        }
    }

    return (
        <dialog
            ref={dialogRef}
            className="sign-dialog"
            aria-labelledby="buy-dialog-title"
            onClose={onClose}
        >
            <h2 id="buy-dialog-title" className="sign-dialog-title">
                Buy {productName}
            </h2>

            <p className="sign-dialog-text">
                From {listing.username ?? listing.userId}
                <br />
                {price.toFixed(2)} kr each
                <br />
                {quantity} in stock
            </p>

            <AmountPicker
                value={amount}
                onChange={setAmount}
                max={quantity}
            />

            <p className="sign-dialog-text">
                <strong>Total: {total.toFixed(2)} kr</strong>
            </p>

            {error && (
                <p className="sign-dialog-error" role="alert">
                    {error}
                </p>
            )}

            <div className="sign-dialog-actions">
                <button
                    type="button"
                    className="stone-button"
                    onClick={() => dialogRef.current?.close()}
                >
                    Cancel
                </button>

                <button
                    type="button"
                    className="stone-button"
                    onClick={handleBuy}
                >
                    Buy
                </button>
            </div>
        </dialog>
    );
}