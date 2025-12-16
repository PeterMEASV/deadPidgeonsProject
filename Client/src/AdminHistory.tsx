import { useEffect, useState } from "react";
import { historyClient } from "./baseUrl";
import type { Historylog } from "./generated-ts-client";

function AdminHistory() {
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
      <div className="overflow-x-auto mt-6">
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