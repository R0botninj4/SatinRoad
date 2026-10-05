import { useEffect, useRef, useState } from "react";
import type { ListingResponseDto } from "../api/Api";
import { api } from "../Auth/apiClient";
import { AmountPicker } from "./AmountPicker";
import "./Dialog.css";

type BuyDialogProps = {
    listing: ListingResponseDto;
    productName: string;
    onClose: () => void;
    onBought: () => void;
};

type Quote = {
    amount: number;
    totalPrice: number;
    discountApplied: boolean;
};

export function BuyDialog({
                              listing,
                              productName,
                              onClose,
                              onBought,
                          }: BuyDialogProps) {
    const dialogRef = useRef<HTMLDialogElement>(null);
    const [amount, setAmount] = useState(1);
    const [quote, setQuote] = useState<Quote | null>(null);
    const [quoteError, setQuoteError] = useState("");
    const [error, setError] = useState("");
    const [buying, setBuying] = useState(false);

    useEffect(() => {
        dialogRef.current?.showModal();
    }, []);

    useEffect(() => {
        if (!listing.id) return;

        let active = true;
        setQuote(null);
        setQuoteError("");

        api.api.orderGetQuote({
            ListingId: listing.id,
            Quantity: amount,
        })
            .then((result) => {
                if (active && result.totalPrice !== undefined) {
                    setQuote({
                        amount,
                        totalPrice: result.totalPrice,
                        discountApplied: result.discountApplied ?? false,
                    });
                }
            })
            .catch(() => {
                if (active) setQuoteError("Could not load the current price.");
            });

        return () => {
            active = false;
        };
    }, [listing.id, amount]);

    const price = listing.price ?? 0;
    const quantity = listing.quantity ?? 0;
    const currentQuote = quote?.amount === amount ? quote : null;

    async function handleBuy() {
        if (!listing.id || !currentQuote || buying) return;

        setBuying(true);
        setError("");

        try {
            await api.api.orderCreate({
                listingId: listing.id,
                quantity: amount,
            });

            dialogRef.current?.close();
            onBought();
        } catch {
            setError("Could not complete the purchase.");
        } finally {
            setBuying(false);
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
                {currentQuote ? (
                    <>
                        <strong>
                            Total: {currentQuote.totalPrice.toFixed(2)} kr
                        </strong>
                        {currentQuote.discountApplied && (
                            <>
                                <br />
                                20% loyalty discount applied
                            </>
                        )}
                    </>
                ) : (
                    "Calculating price..."
                )}
            </p>

            {quoteError && (
                <p className="sign-dialog-error" role="alert">
                    {quoteError}
                </p>
            )}

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
                    disabled={!currentQuote || buying}
                >
                    {buying ? "Buying..." : "Buy"}
                </button>
            </div>
        </dialog>
    );
}