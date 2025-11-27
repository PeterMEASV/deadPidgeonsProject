import { useEffect, useState } from "react";

const API_BASE = "http://localhost:5099/api"; // change if needed

export default function PlayerNewGame() {
    const [toggledButtons, setToggledButtons] = useState<Set<number>>(new Set());
    const [showConfirmation, setShowConfirmation] = useState(false);
    const [price, setPrice] = useState<number | null>(null);
    const [loadingPrice, setLoadingPrice] = useState(false);
    const [submitState, setSubmitState] = useState<{ loading: boolean; error?: string; success?: string }>({
        loading: false,
    });

    // --- Toggle tiles, max 8 allowed ---
    const handleButtonClick = (index: number) => {
        setToggledButtons(prev => {
            const newSet = new Set(prev);
            if (newSet.has(index)) {
                newSet.delete(index);
            } else {
                if (newSet.size >= 8) return newSet; // enforce max 8
                newSet.add(index);
            }
            return newSet;
        });
    };

    // --- Fetch price from backend whenever selected numbers change ---
    useEffect(() => {
        const selectedNumbers = Array.from(toggledButtons).map(i => i + 1);

        if (selectedNumbers.length < 5) {
            setPrice(null);
            setLoadingPrice(false);  // Ensure it's reset if <5
            return;
        }

        setLoadingPrice(true);
        const abortController = new AbortController();  // New: For cancelling the fetch

        const t = setTimeout(() => {
            fetch(`${API_BASE}/board/validate`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(selectedNumbers),
                credentials: "include",  // Ensure auth is included (as in your submit logic)
                signal: abortController.signal  // New: Attach the abort signal
            })
                .then(async res => {
                    if (!res.ok) throw new Error(await res.text());
                    return res.json();
                })
                .then(data => {
                    if (!data.isValid) {
                        setPrice(null);
                        return;
                    }
                    const p = data.price ?? data.Price;  // Handles backend's capital "Price"
                    setPrice(p);
                })
                .catch(err => {
                    if (err.name === 'AbortError') return;  // New: Ignore aborted fetches
                    console.error(err);
                    setPrice(null);
                })
                .finally(() => {
                    setLoadingPrice(false);  // Always reset loading state
                });
        }, 150);

        return () => {
            abortController.abort();  // New: Abort the fetch on cleanup
            clearTimeout(t);
        };
    }, [toggledButtons]);



    // --- Show confirmation modal ---
    const handleEndGame = () => {
        setShowConfirmation(true);
    };

    // --- Submitting board to backend ---
    const submitBoard = async () => {
        const selectedNumbers = Array.from(toggledButtons).map(i => i + 1);

        if (selectedNumbers.length < 5 || selectedNumbers.length > 8) {
            setSubmitState({ loading: false, error: "vælg 5-8 numre." });
            return;
        }

        try {
            const payload = {
                selectedNumbers,
                repeatForWeeks: 1
            };

            const res = await fetch(`${API_BASE}/board/create`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(payload),
                credentials: "include"
            });

            if (!res.ok) throw new Error(await res.text());

            await res.json();

            setSubmitState({ loading: false, success: "spilleplade købt!" });
            setToggledButtons(new Set());
            setPrice(null);

        } catch (err: any) {
            setSubmitState({ loading: false, error: err.message || "kunne ikke købe spilleplade." });
        }
    };

    // --- Confirm button inside modal ---
    const handleConfirmSubmit = async () => {
        setShowConfirmation(false);
        setSubmitState({ loading: true });
        await submitBoard();
    };

    // --- Cancels confirm modal ---
    const handleCancel = () => setShowConfirmation(false);

    const tileContent = (index: number) => {
        if (toggledButtons.has(index)) {
            return (
                <div className="w-16 h-16 rounded-full bg-white flex items-center justify-center text-[#E50006FF] font-bold">
                    {index + 1}
                </div>
            );
        }
        return index + 1;
    };

    const selectionCount = toggledButtons.size;

    return (
        <>
            <div className="grid grid-cols-4 gap-4 w-full p-4">
                {Array.from({ length: 16 }).map((_, index) => (
                    <button
                        key={index}
                        className={`btn h-32 text-3xl flex items-center justify-center ${
                            toggledButtons.has(index)
                                ? "bg-[#E50006FF] text-white hover:bg-[#AF0006FF]"
                                : "bg-[#F44336] text-white hover:bg-[#c93232]"
                        }`}
                        onClick={() => handleButtonClick(index)}
                    >
                        {tileContent(index)}
                    </button>
                ))}
            </div>

            <div className="flex justify-center mt-6 mb-4">
                <button
                    className="btn bg-[#E50006FF] text-white text-xl px-8 py-4 h-auto hover:bg-[#AF0006FF]"
                    disabled={selectionCount < 5 || selectionCount > 8}
                    onClick={handleEndGame}
                >
                    {loadingPrice
                        ? "Checker pris…"
                        : price != null
                            ? `${price} DKK`
                            : "vælg 5-8 numre."}
                </button>
            </div>

            {submitState.error && (
                <p className="text-center text-red-600 mb-4">{submitState.error}</p>
            )}
            {submitState.success && (
                <p className="text-center text-green-600 mb-4">{submitState.success}</p>
            )}

            {/* CONFIRMATION MODAL */}
            {showConfirmation && (
                <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
                    <div className="bg-white rounded-lg p-6 max-w-md w-full">
                        <h2 className="text-xl font-bold mb-4">bekræft køb</h2>
                        <p>spillepladen koster {price} DKK. fortsæt?</p>

                        <div className="flex justify-end gap-4 mt-4">
                            <button className="btn btn-ghost px-6 py-3" onClick={handleCancel}>
                                annuller
                            </button>

                            <button
                                className="btn bg-[#E50006FF] text-white px-6 py-3 hover:bg-[#AF0006FF]"
                                onClick={handleConfirmSubmit}
                            >
                                bekræft
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </>
    );
}