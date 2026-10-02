import { useEffect, useRef, useState, type FormEvent } from "react";
import { AmountPicker } from "./AmountPicker";
import "./Dialog.css";

export type NewListing = {
    amount: number;
    pricePerItem: number;
};

type SellDialogProps = {
    productName: string;
    onSubmit: (listing: NewListing) => void;
    onClose: () => void;
};

// A positive price with at most 2 decimals, e.g. "5", "5.5", "5,50"
const PRICE_PATTERN = /^\d+([.,]\d{1,2})?$/;

// Dialog where a logged-in user creates a listing for one product.
export function SellDialog({ productName, onSubmit, onClose }: SellDialogProps) {
    const dialogRef = useRef<HTMLDialogElement>(null);
    const [amount, setAmount] = useState(1);
    const [priceText, setPriceText] = useState("");

    // Open as a modal as soon as the component is shown
    useEffect(() => {
        dialogRef.current?.showModal();
    }, []);

    // Accept both "5.50" and the Danish "5,50"
    const price = Number(priceText.replace(",", "."));
    const priceIsValid = PRICE_PATTERN.test(priceText) && price > 0;
    const showPriceError = priceText !== "" && !priceIsValid;

    function handleSubmit(event: FormEvent) {
        event.preventDefault();
        if (!priceIsValid) return;
        onSubmit({ amount, pricePerItem: price });
        dialogRef.current?.close();
    }

    return (
        <dialog
            ref={dialogRef}
            className="sign-dialog"
            aria-labelledby="sell-dialog-title"
            onClose={onClose}
        >
            <form onSubmit={handleSubmit} noValidate>
                <h2 id="sell-dialog-title" className="sign-dialog-title">
                    Sell {productName}
                </h2>

                <div className="sign-dialog-field">
                    <span>Amount</span>
                    <AmountPicker value={amount} onChange={setAmount} />
                </div>

                <label className="sign-dialog-field">
                    <span>Price per item (kr)</span>
                    <input
                        className="sign-dialog-input"
                        type="text"
                        inputMode="decimal"
                        placeholder="0.00"
                        value={priceText}
                        onChange={(event) => setPriceText(event.target.value.trim())}
                        aria-invalid={showPriceError}
                        aria-describedby="sell-dialog-error"
                    />
                </label>

                <p id="sell-dialog-error" className="sign-dialog-error" role="alert">
                    {showPriceError && "Enter a price above 0 with at most 2 decimals."}
                </p>

                <p className="sign-dialog-text">
                    {priceIsValid
                        ? `${amount} × ${price.toFixed(2)} kr = ${(amount * price).toFixed(2)} kr`
                        : "\u00A0"}
                </p>

                <div className="sign-dialog-actions">
                    <button type="button" className="stone-button" onClick={() => dialogRef.current?.close()}>
                        Cancel
                    </button>
                    <button type="submit" className="stone-button" disabled={!priceIsValid}>
                        List for sale
                    </button>
                </div>
            </form>
        </dialog>
    );
}