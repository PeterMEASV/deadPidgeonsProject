import { useState } from "react";
import { useAtomValue } from "jotai";
import { useNavigate } from "react-router";
import { userInfoAtom } from "./Token.tsx";
import { balanceClient } from "./baseUrl";

export default function PlayerAddFunds() {
    const navigate = useNavigate();
    const user = useAtomValue(userInfoAtom);

    const [amount, setAmount] = useState<number>(0);
    const [transactionNumber, setTransactionNumber] = useState("");
    const [state, setState] = useState<{
        loading: boolean;
        error?: string;
        success?: string;
    }>({ loading: false });

    const submitDeposit = async () => {
        if (!user?.id) {
            setState({ loading: false, error: "Du er ikke logget ind." });
            return;
        }

        if (amount <= 0) {
            setState({ loading: false, error: "Indtast et gyldigt beløb." });
            return;
        }

        if (!transactionNumber.trim()) {
            setState({ loading: false, error: "Indtast MobilePay transaktionsnummer." });
            return;
        }

        try {
            setState({ loading: true });

            await balanceClient.submitDeposit({
                userId: user.id,
                amount: amount,
                transactionnumber: transactionNumber,
            });

            setState({
                loading: false,
                success: "Indbetaling sendt til godkendelse hos admin.",
            });

            setAmount(0);
            setTransactionNumber("");
        } catch (err: any) {
            setState({
                loading: false,
                error:
                    err?.message ||
                    err?.response ||
                    "Kunne ikke indsætte penge.",
            });
        }
    };

    return (
        <div className="p-6 max-w-lg mx-auto">
            <h1 className="text-3xl font-bold mb-6">Indsæt penge</h1>

            <div className="space-y-4">
                <div>
                    <label className="block font-semibold mb-1">Beløb (DKK)</label>
                    <input
                        type="number"
                        min={1}
                        value={amount}
                        onChange={e => setAmount(Number(e.target.value))}
                        className="input input-bordered w-full"
                    />
                </div>

                <div>
                    <label className="block font-semibold mb-1">
                        MobilePay transaktionsnummer
                    </label>
                    <input
                        type="text"
                        value={transactionNumber}
                        onChange={e => setTransactionNumber(e.target.value)}
                        className="input input-bordered w-full"
                        placeholder="Fx MP123456789"
                    />
                </div>

                {state.error && (
                    <p className="text-red-600">{state.error}</p>
                )}

                {state.success && (
                    <p className="text-green-600">{state.success}</p>
                )}

                <div className="flex justify-end gap-4 pt-4">
                    <button
                        className="btn btn-ghost"
                        onClick={() => navigate("/Player/Transactions")}
                    >
                        Tilbage
                    </button>

                    <button
                        className="btn bg-[#E50006FF] text-white hover:bg-[#AF0006FF]"
                        disabled={state.loading}
                        onClick={submitDeposit}
                    >
                        {state.loading ? "Sender…" : "Indsæt penge"}
                    </button>
                </div>
            </div>
        </div>
    );
}
