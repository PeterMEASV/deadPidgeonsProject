import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router";
import { boardClient } from "./baseUrl.ts";
import type { BoardHistoryResponseDTO } from "./generated-ts-client.ts";

function AdminBoardsForGame() {
    const { gameId } = useParams<{ gameId: string }>();
    const navigate = useNavigate();
    const [boards, setBoards] = useState<BoardHistoryResponseDTO[]>([]);

    useEffect(() => {
        if (gameId) {
            boardClient.getBoardForGame(gameId).then((r) => {
                console.log('Boards for game response:', r);
                setBoards(r ?? []);
            });
        }
    }, [gameId]);

    const formatDate = (timestamp?: string) => {
        if (!timestamp) return "N/A";
        return new Date(timestamp).toLocaleString("da-DK");
    };

    const formatNumbers = (numbers?: number[]) => {
        if (!numbers || numbers.length === 0) return "N/A";
        return numbers.join(", ");
    };

    return (
        <>
            <div className="flex justify-center gap-3 mt-6 mb-4">
                <button className="btn bg-[#E50006FF] text-white px-6 py-2 hover:bg-[#AF0006FF]"
                    onClick={() => navigate('/admin/GameHistory')}>← Back to Game History</button>
            </div>

            <div className="flex justify-center gap-3 mb-4">
                <h2 className="text-2xl font-bold">Boards for Game</h2>
            </div>

            <div className="overflow-x-auto">
                <table className="table">
                    <thead>
                        <tr className="text-center bg-[#bfbfbd]">
                            <th>Name</th>
                            <th>Phone Number</th>
                            <th>Selected Numbers</th>
                            <th>Timestamp</th>
                            <th>Winner</th>
                            <th>Repeat</th>
                        </tr>
                    </thead>
                    <tbody>
                        {boards.length === 0 ? (
                            <tr>
                                <td colSpan={6} className="text-center py-8 text-gray-500">
                                    No boards found for this game
                                </td>
                            </tr>
                        ) : (
                            boards.map((board, index) => (
                                <tr
                                    key={board.id ?? index}
                                    className={`text-center ${index % 2 !== 0 ? 'bg-[#bfbfbd]' : ''}`}>
                                    <td>{board.userName ?? "N/A"}</td>
                                    <td>{board.userPhone ?? "N/A"}</td>
                                    <td>{formatNumbers(board.selectedNumbers)}</td>
                                    <td>{formatDate(board.timestamp)}</td>
                                    <td>
                                        <span className={`badge ${board.winner ? 'badge-success' : 'badge-neutral'}`}>
                                            {board.winner ? "Yes" : "No"}
                                        </span>
                                    </td>
                                    <td>
                                        <span className={`badge ${board.repeat ? 'badge-success' : 'badge-neutral'}`}>
                                            {board.repeat ? "Yes" : "No"}
                                        </span>
                                    </td>
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>
        </>
    );
}

export default AdminBoardsForGame;