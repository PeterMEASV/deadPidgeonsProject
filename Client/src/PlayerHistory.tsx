import { useEffect, useState } from "react";
import { useAtomValue } from "jotai";
import { userInfoAtom } from "./Token.tsx";
import { historyClient } from "./baseUrl";

interface BoardHistoryDTO {
    boardId: string;
    selectedNumbers: number[];
    weeknumber: string;
    winner: boolean;
    winningNumbers?: number[];
    drawDate: string;
}


export default function PlayerHistory() {
    const user = useAtomValue(userInfoAtom);

    const [history, setHistory] = useState<BoardHistoryDTO[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!user?.id) {
            setError("Du er ikke logget ind.");
            setLoading(false);
            return;
        }

        historyClient
            .getUserBoardHistory(user.id)
            .then(r => {
                setHistory(r);
                console.log('History:', r);
            })
            .catch(() => {
                setError("Kunne ikke hente spilhistorik.");
            })
            .finally(() => {
                setLoading(false);
            });
    }, [user]);

    const formatDate = (timestamp?: string) => {
        if (!timestamp) return "N/A";
        return new Date(timestamp).toLocaleString("da-DK");
    };

    return (
        <div className="p-6">
            <h1 className="text-3xl font-bold mb-6">Spilhistorik</h1>

            {loading && <p>Loading…</p>}
            {error && <p className="text-red-600">{error}</p>}

            <div className="overflow-x-auto">
                <table className="table">
                    <thead>
                    <tr className="text-center bg-[#bfbfbd]">
                        <th>Board</th>
                        <th>Valgte numre</th>
                        <th>Uge</th>
                        <th>Status</th>
                        <th>Vindertal</th>
                        <th>Dato</th>
                    </tr>
                    </thead>
                    <tbody>
                    {history.length === 0 && !loading ? (
                        <tr>
                            <td colSpan={6} className="text-center py-8 text-gray-500">
                                Ingen spilhistorik fundet
                            </td>
                        </tr>
                    ) : (
                        history.map((h, index) => (
                            <tr
                                key={`${h.boardId}-${index}`}
                                className={`text-center ${
                                    index % 2 !== 0 ? "bg-[#bfbfbd]" : ""
                                }`}
                            >
                                <td>{h.boardId.slice(0, 8)}</td>
                                <td>{h.selectedNumbers.join(", ")}</td>
                                <td>{h.weeknumber || "N/A"}</td>

                                <td
                                    className={
                                        h.winner
                                            ? "text-green-600 font-semibold"
                                            : "text-gray-500"
                                    }
                                >
                                    {h.winner ? "Vundet" : "Tabt"}
                                </td>
                                <td>
                                    {h.winningNumbers && h.winningNumbers.length > 0
                                        ? h.winningNumbers.join(", ")
                                        : "-"}
                                </td>
                                <td>{formatDate(h.drawDate)}</td>
                            </tr>
                        ))
                    )}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
