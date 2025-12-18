import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { gameClient } from "./baseUrl.ts";
import type { GameResponseDTO } from "./generated-ts-client.ts";

function AdminGameHistory() {
    const [games, setGames] = useState<GameResponseDTO[]>([]);
    const navigate = useNavigate();

    useEffect(() => {
        gameClient.getGameHistory().then((r) => {
            console.log('Game history response:', r);
            setGames(r);
        });
    }, []);

    const formatDate = (game: GameResponseDTO) => {
        const dateValue = (game as any).drawDate ?? (game as any).drawdate;
        if (!dateValue) return "N/A";
        return new Date(dateValue).toLocaleString("da-DK");
    };

    const formatWinningNumbers = (game: GameResponseDTO) => {
        const numbers = (game as any).winningNumbers ?? (game as any).winningnumbers;
        if (!numbers || numbers.length === 0) return "Not drawn yet";
        return numbers.join(", ");
    };

    const getIsActive = (game: GameResponseDTO) => {
        return (game as any).isActive ?? (game as any).isactive ?? false;
    };

    const handleGameClick = (game: GameResponseDTO) => {
        if (game.id) {
            navigate(`/admin/GameHistory/${game.id}`);
        }
    };

    return (
        <>
            <div className="flex justify-center gap-3 mt-6 mb-4">
                <h2 className="text-2xl font-bold">Game History</h2>
            </div>

            <div className="overflow-x-auto">
                <table className="table">
                    <thead>
                        <tr className="text-center bg-[#bfbfbd]">
                            <th>Week</th>
                            <th>Draw Date</th>
                            <th>Winning Numbers</th>
                            <th>Status</th>
                            <th>Total Boards</th>
                            <th>Winners</th>
                        </tr>
                    </thead>
                    <tbody>
                        {games.length === 0 ? (
                            <tr>
                                <td colSpan={6} className="text-center py-8 text-gray-500">
                                    No games found
                                </td>
                            </tr>
                        ) : (
                            games.map((game, index) => (
                                <tr
                                    key={game.id ?? index}
                                    className={`hover:bg-base-300 cursor-pointer text-center ${index % 2 !== 0 ? 'bg-[#bfbfbd]' : ''}`}
                                    onClick={() => handleGameClick(game)}>
                                    <td>Week {game.weeknumber ?? "N/A"}</td>
                                    <td>{formatDate(game)}</td>
                                    <td>{formatWinningNumbers(game)}</td>
                                    <td>
                                        <span className={`badge ${getIsActive(game) ? 'badge-success' : 'badge-neutral'}`}>
                                            {getIsActive(game) ? "Active" : "Completed"}
                                        </span>
                                    </td>
                                    <td>{game.totalBoards ?? 0}</td>
                                    <td>{game.totalWinners ?? 0}</td>
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>
        </>
    );
}

export default AdminGameHistory;