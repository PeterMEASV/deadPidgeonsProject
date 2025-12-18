import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { useAtomValue } from "jotai";
import { userInfoAtom } from "./Token.tsx";
import { balanceClient } from "./baseUrl";

interface PlayerTransaction {
    id: string;
    amount: number;
    transactionNumber?: string;
    timestamp: string;
    status?: string; // pending / approved
    type?: string;   // topup / board / etc (if backend provides it)
}

export default function PlayerTransactions() {
    const navigate = useNavigate();
    const user = useAtomValue(userInfoAtom);

    const [transactions, setTransactions] = useState<PlayerTransaction[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!user?.id) {
            setError("Du er ikke logget ind.");
            setLoading(false);
            return;
        }

        const userId = user.id;

        const fetchTransactions = async () => {
            try {
                // SAME PATTERN AS PlayerGame
                const result = await balanceClient.getUserTransactions(userId);
                setTransactions(result);
            } catch (err) {
                setError("Kunne ikke hente dine transaktioner.");
            } finally {
                setLoading(false);
            }
        };

        fetchTransactions();
    }, [user]);

    const formatDate = (timestamp?: string) => {
        if (!timestamp) return "N/A";

        const d = new Date(timestamp);
        if (isNaN(d.getTime())) return "Ugyldig dato";

        return d.toLocaleString("da-DK");
    };

    return (
        <div className="p-6">

            {/* HEADER */}
            <div className="flex justify-between items-center mb-6">
                <h1 className="text-3xl font-bold">dine transaktioner</h1>

                <button
                    className="btn bg-[#E50006FF] text-white hover:bg-[#AF0006FF] px-6 py-3"
                    onClick={() => navigate("/player/add")}
                >
                    Indsæt penge
                </button>
            </div>

            {loading && <p>Loading...</p>}
            {error && <p className="text-red-600">{error}</p>}

            <div className="overflow-x-auto">
                <table className="table">
                    <thead>
                    <tr className="text-center bg-[#bfbfbd]">
                        <th>Transaktion</th>
                        <th>Type</th>
                        <th>Beløb</th>
                        <th>Status</th>
                        <th>Tidspunkt</th>
                    </tr>
                    </thead>

                    <tbody>
                    {transactions.length === 0 && !loading ? (
                        <tr>
                            <td colSpan={5} className="text-center py-8 text-gray-500">
                                du har ingen transaktioner endnu
                            </td>
                        </tr>
                    ) : (
                        transactions.map((t, index) => (
                            <tr
                                key={t.id}
                                className={`text-center ${index % 2 !== 0 ? "bg-[#bfbfbd]" : ""}`}
                            >
                                <td>
                                    {t.transactionNumber ?? String(t.id)}
                                </td>
                                <td>{t.type ?? "—"}</td>
                                <td>{t.amount} kr</td>
                                <td>{t.status ?? "approved"}</td>
                                <td>{formatDate(t.timestamp)}</td>
                            </tr>
                        ))
                    )}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
