import { useEffect, useRef, useState } from "react";
import type { Listing } from "../TempData/Mockdata.ts";
import { AmountPicker } from "./AmountPicker";
import "./Dialog.css";

type BuyDialogProps = {
    listing: Listing;
    productName: string;
    onClose: () => void;
};

// Dialog where the buyer picks how many items to buy from one listing.
export function BuyDialog({ listing, productName, onClose }: BuyDialogProps) {
    const dialogRef = useRef<HTMLDialogElement>(null);
    const [amount, setAmount] = useState(1);

    // Open as a modal as soon as the component is shown
    useEffect(() => {
        dialogRef.current?.showModal();
    }, []);

    const total = amount * listing.pricePerItem;

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
                From {listing.vendor} : {listing.pricePerItem.toFixed(2)} kr each
                <br />
                {listing.quantity} in stock
            </p>

            <AmountPicker value={amount} onChange={setAmount} max={listing.quantity} />

            <p className="sign-dialog-text">
                <strong>Total: {total.toFixed(2)} kr</strong>
            </p>

            <div className="sign-dialog-actions">
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