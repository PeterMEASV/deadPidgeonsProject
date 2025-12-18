import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { historyClient } from "./baseUrl";
import type { Historylog } from "./generated-ts-client";

function AdminHistory() {
  const navigate = useNavigate();
  const [logs, setLogs] = useState<Historylog[]>([]);

  useEffect(() => {
    historyClient.getAllLogs().then((r) => {
      setLogs(r);
    });
  }, []);

  const formatDate = (timestamp?: string) => {
    if (!timestamp) return "N/A";
    return new Date(timestamp).toLocaleString("da-DK");
  };

  return (
    <>
      <div className="flex justify-center gap-3 mt-6 mb-4">
        <button
          className="btn bg-[#E50006FF] text-white text-xl px-8 py-4 h-auto hover:bg-[#AF0006FF]"
          onClick={() => navigate('/admin/GameHistory')}
        >
          Game History
        </button>
      </div>

      <div className="overflow-x-auto">
        <table className="table">
          <thead>
            <tr className="text-center bg-[#bfbfbd]">
              <th>Content</th>
              <th>Timestamp</th>
            </tr>
          </thead>
          <tbody>
            {logs.length === 0 ? (
              <tr>
                <td colSpan={2} className="text-center py-8 text-gray-500">
                  No history logs found
                </td>
              </tr>
            ) : (
              logs.map((log, index) => (
                <tr
                  key={log.id ?? `${log.timestamp ?? "no-ts"}-${index}`}
                  className={`text-center ${index % 2 !== 0 ? "bg-[#bfbfbd]" : ""}`}
                >
                  <td className="whitespace-pre-wrap break-words">{log.content ?? ""}</td>
                  <td>{formatDate(log.timestamp)}</td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </>
  );
}

export default AdminHistory;