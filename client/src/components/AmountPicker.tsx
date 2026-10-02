import "./AmountPicker.css";

type AmountPickerProps = {
    value: number;
    onChange: (value: number) => void;
    max?: number; // leave out for no upper limit
};

// "- [ amount ] +" control. Always keeps the value between 1 and max.
export function AmountPicker({ value, onChange, max = Infinity }: AmountPickerProps) {
    function change(next: number) {
        if (Number.isNaN(next)) return;
        onChange(Math.min(Math.max(next, 1), max));
    }

    return (
        <div className="amount-picker">
            <button
                type="button"
                className="stone-button"
                onClick={() => change(value - 1)}
                disabled={value <= 1}
                aria-label="One less"
            >
                -
            </button>

            <input
                className="sign-dialog-input amount-picker-input"
                type="number"
                min={1}
                max={Number.isFinite(max) ? max : undefined}
                value={value}
                onChange={(event) => change(event.target.valueAsNumber)}
                aria-label="Amount"
            />

            <button
                type="button"
                className="stone-button"
                onClick={() => change(value + 1)}
                disabled={value >= max}
                aria-label="One more"
            >
                +
            </button>
        </div>
    );
}