import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { gameClient } from "./baseUrl.ts";
import type { GameResponseDTO } from "./generated-ts-client.ts";

function AdminGameHistory() {
    const [games, setGames] = useState<GameResponseDTO[]>([]);
    const navigate = useNavigate();

    useEffect(() => {
        void gameClient.getGameHistory()
            .then((r) => {
                console.log('Game history response:', r);
                setGames(r);
            })
            .catch((error) => {
                console.error('Failed to load game history:', error);
            });
    }, []);

    const formatDate = (game: GameResponseDTO): string => {
        const dateValue = (game as Record<string, unknown>).drawDate ?? (game as Record<string, unknown>).drawdate;
        if (!dateValue) return "N/A";
        return new Date(dateValue as string).toLocaleString("da-DK");
    };

    const formatWinningNumbers = (game: GameResponseDTO): string => {
        const numbers = (game as Record<string, unknown>).winningNumbers ?? (game as Record<string, unknown>).winningnumbers;
        if (!numbers || !Array.isArray(numbers) || numbers.length === 0) return "Not drawn yet";
        return numbers.join(", ");
    };

    const getIsActive = (game: GameResponseDTO): boolean => {
        return ((game as Record<string, unknown>).isActive ?? (game as Record<string, unknown>).isactive ?? false) as boolean;
    };

    const handleGameClick = async (game: GameResponseDTO): Promise<void> => {
        if (game.id) {
            await navigate(`/admin/GameHistory/${game.id}`);
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