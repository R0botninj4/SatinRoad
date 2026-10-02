import { useEffect, useRef, useState } from "react";
import type { Listing } from "../TempData/Mockdata.ts";
import "./BuyDialog.css";

type BuyDialogProps = {
    listing: Listing;
    productName: string;
    onClose: () => void;
};

// Pop-up where the buyer picks how many items to buy from one listing.
export function BuyDialog({ listing, productName, onClose }: BuyDialogProps) {
    const dialogRef = useRef<HTMLDialogElement>(null);
    const [amount, setAmount] = useState(1);

    // Open as a modal as soon as the component is shown
    useEffect(() => {
        dialogRef.current?.showModal();
    }, []);

    // Keep the amount between 1 and what the vendor has in stock
    function changeAmount(value: number) {
        if (Number.isNaN(value)) return;
        setAmount(Math.min(Math.max(value, 1), listing.quantity));
    }

    const total = amount * listing.pricePerItem;

    return (
        <dialog
            ref={dialogRef}
            className="buy-dialog"
            aria-labelledby="buy-dialog-title"
            onClose={onClose}
        >
            <h2 id="buy-dialog-title" className="buy-dialog-title">
                Buy {productName}
            </h2>

            <p className="buy-dialog-details">
                From {listing.vendor} · {listing.pricePerItem.toFixed(2)} kr each
                <br />
                {listing.quantity} in stock
            </p>

            <div className="buy-dialog-amount">
                <button
                    type="button"
                    className="stone-button"
                    onClick={() => changeAmount(amount - 1)}
                    disabled={amount <= 1}
                    aria-label="One less"
                >
                    -
                </button>

                <input
                    className="buy-dialog-input"
                    type="number"
                    min={1}
                    max={listing.quantity}
                    value={amount}
                    onChange={(event) => changeAmount(event.target.valueAsNumber)}
                    aria-label="Amount"
                />

                <button
                    type="button"
                    className="stone-button"
                    onClick={() => changeAmount(amount + 1)}
                    disabled={amount >= listing.quantity}
                    aria-label="One more"
                >
                    +
                </button>
            </div>

            <p className="buy-dialog-total">Total: {total.toFixed(2)} kr</p>

            <div className="buy-dialog-actions">
                <button type="button" className="stone-button" onClick={() => dialogRef.current?.close()}>
                    Cancel
                </button>
                {/* TODO: send the purchase to the backend */}
                <button type="button" className="stone-button" onClick={() => dialogRef.current?.close()}>
                    Buy
                </button>
            </div>
        </dialog>
    );
}