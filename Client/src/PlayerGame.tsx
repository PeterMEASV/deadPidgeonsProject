import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { useAtomValue } from "jotai";
import { userInfoAtom } from "./Token.tsx";
import { boardClient } from "./baseUrl";
import type { BoardResponseDTO } from "./generated-ts-client";

interface PlayerBoard {
  id: string;
  selectedNumbers: number[];
  price: number;
  timestamp: string;
}

const toPlayerBoard = (dto: BoardResponseDTO): PlayerBoard | null => {
  if (!dto.id) return null;

  return {
    id: dto.id,
    selectedNumbers: dto.selectedNumbers ?? [],
    price: dto.price ?? 0,
    timestamp: dto.timestamp ?? "",
  };
};

export default function PlayerGame() {
  const navigate = useNavigate();
  const user = useAtomValue(userInfoAtom);

  const [boards, setBoards] = useState<PlayerBoard[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const userId = user?.id;

    if (!userId) {
      setError("Du er ikke logget ind.");
      setLoading(false);
      return;
    }

    const fetchBoards = async () => {
      try {
        const result = await boardClient.getBoardsByUser(userId);

        const mapped = result
          .map(toPlayerBoard)
          .filter((b): b is PlayerBoard => b !== null);

        setBoards(mapped);
      } catch (err) {
        setError("kunne ikke finde igangværende spilleplader.");
      } finally {
        setLoading(false);
      }
    };

    void fetchBoards();
  }, [user?.id]);

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

      {loading && <p>Loading...</p>}
      {error && <p className="text-red-600">{error}</p>}

      <div className="overflow-x-auto">
        <table className="table">
          <thead>
            <tr className="text-center bg-[#bfbfbd]">
              <th>Board ID</th>
              <th>Numbers</th>
              <th>Price</th>
              <th>Created</th>
            </tr>
          </thead>

          <tbody>
            {boards.length === 0 && !loading ? (
              <tr>
                <td colSpan={4} className="text-center py-8 text-gray-500">
                  du har ingen igangværende spilleplader
                </td>
              </tr>
            ) : (
              boards.map((board, index) => (
                <tr
                  key={board.id}
                  className={`text-center ${index % 2 !== 0 ? "bg-[#bfbfbd]" : ""}`}
                >
                  <td>{board.id.slice(0, 8)}</td>
                  <td>{board.selectedNumbers.join(", ")}</td>
                  <td>{board.price} DKK</td>
                                <td>{new Date(board.timestamp).toLocaleString("da-DK")}</td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
