import { useEffect, useState } from "react";
import {useNavigate} from "react-router";

const API_BASE = "http://localhost:5099/api"; // update if needed

interface PlayerBoard {
    id: string;
    selectedNumbers: number[];
    price: number;
    createdAt: string;
}

export default function PlayerGame() {


    const [boards, setBoards] = useState<PlayerBoard[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate()

    useEffect(() => {
        const fetchBoards = async () => {
            try {
                const res = await fetch(`${API_BASE}/board/myboards`, {
                    credentials: "include",
                });
                if (!res.ok) throw new Error(await res.text());

                const data = await res.json();
                setBoards(data);
            } catch (err: any) {
                setError("kunne ikke finde igangværende spilleplader.");
            } finally {
                setLoading(false);
            }
        };

        fetchBoards();
    }, []);

    return (
        <div className="p-6">

            {/* HEADER */}
            <div className="flex justify-between items-center mb-6">
                <h1 className="text-3xl font-bold">dine spil</h1>

                <button
                    className="btn bg-[#E50006FF] text-white hover:bg-[#AF0006FF] px-6 py-3 text-lg"
                    onClick={() => navigate("/Player/NewGame")}
                >
                    køb ny spilleplade
                </button>
            </div>

            {/* LOADING / ERROR */}
            {loading && <p>Loading...</p>}
            {error && <p className="text-red-600">{error}</p>}

            {/* BOARD LIST */}
            {boards.length === 0 && !loading && (
                <p className="text-gray-500 text-lg">du har ingen igangværende spilleplader</p>
            )}

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {boards.map(board => (
                    <div key={board.id} className="p-4 bg-white rounded-lg shadow">

                        <h2 className="text-xl font-semibold mb-2">Board #{board.id.slice(0, 8)}</h2>

                        <p className="mb-1">
                            <strong>Numbers:</strong> {board.selectedNumbers.join(", ")}
                        </p>

                        <p className="mb-1">
                            <strong>Price:</strong> {board.price} DKK
                        </p>

                        <p className="text-sm text-gray-500">
                            {new Date(board.createdAt).toLocaleString()}
                        </p>
                    </div>
                ))}
            </div>
        </div>
    );
}