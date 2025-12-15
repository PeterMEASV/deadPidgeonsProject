import {useEffect, useState} from "react";
import type {ApproveTransactionDTO} from "./generated-ts-client.ts";
import {balanceClient} from "./baseUrl.ts";


function AdminTransactions() {
    const [transactions, setTransactions] = useState<any[]>([]);
    const [selectedTransaction, setSelectedTransaction] = useState<any | null>(null);
    const [showPending, setShowPending] = useState(true);

    useEffect(() => {
        loadTransactions();
    }, [showPending]);

    const loadTransactions = () => {
        if (showPending) {
            balanceClient.getPendingTransactions().then(r => {
                console.log('Pending transactions:', r);
                setTransactions(r);
            });
        } else {
            balanceClient.getApprovedTransactions().then(r => {
                console.log('Approved transactions:', r);
                setTransactions(r);
            });
        }
    };

    const handleTransactionClick = (e: React.MouseEvent, transaction: any) => {
        e.stopPropagation();
        if (showPending) {
            setSelectedTransaction(transaction);
            (document.getElementById('approve_transaction_modal') as HTMLDialogElement)?.showModal();
        }
    };

    const handleApprove = async () => {
        if (!selectedTransaction?.id) return;

        try {
            const dto: ApproveTransactionDTO = {
                transactionId: selectedTransaction.id
            };
            await balanceClient.approveTransaction(dto);
            loadTransactions();
            (document.getElementById('approve_transaction_modal') as HTMLDialogElement)?.close();
            setSelectedTransaction(null);
        } catch (error) {
            console.error('Failed to approve transaction:', error);
        }
    };

    const handleCancel = () => {
        (document.getElementById('approve_transaction_modal') as HTMLDialogElement)?.close();
        setSelectedTransaction(null);
    };

    const formatDate = (timestamp?: string) => {
        if (!timestamp) return 'N/A';
        return new Date(timestamp).toLocaleString('da-DK');
    };

    return (
        <>
            <div className="flex justify-center gap-3 mt-6 mb-4">
                <button
                    className={`btn text-white text-xl px-8 py-4 h-auto ${showPending ? 'bg-[#E50006FF] hover:bg-[#AF0006FF]' : 'bg-gray-500 hover:bg-gray-600'}`}
                    onClick={() => setShowPending(true)}
                >
                    Pending
                </button>
                <button
                    className={`btn text-white text-xl px-8 py-4 h-auto ${!showPending ? 'bg-[#E50006FF] hover:bg-[#AF0006FF]' : 'bg-gray-500 hover:bg-gray-600'}`}
                    onClick={() => setShowPending(false)}
                >
                    Approved
                </button>
            </div>

            <div className="overflow-x-auto">
                <table className="table">
                    <thead>
                    <tr className="text-center bg-[#bfbfbd]">
                        <th>Transaction Number</th>
                        <th>User ID</th>
                        <th>Amount</th>
                        <th>Timestamp</th>
                    </tr>
                    </thead>
                    <tbody>
                    {transactions.length === 0 ? (
                        <tr>
                            <td colSpan={4} className="text-center py-8 text-gray-500">
                                No {showPending ? 'pending' : 'approved'} transactions found
                            </td>
                        </tr>
                    ) : (
                        transactions.map((transaction, index) => (
                            <tr
                                key={transaction.id}
                                className={`text-center ${index % 2 !== 0 ? 'bg-[#bfbfbd]' : ''} ${showPending ? 'cursor-pointer' : ''}`}
                                onClick={(e) => showPending && handleTransactionClick(e, transaction)}
                            >
                                <td className={showPending ? "hover:bg-base-300" : ""}>{transaction.transactionNumber || transaction.transactionnumber}</td>
                                <td className={showPending ? "hover:bg-base-300" : ""}>{transaction.userId || transaction.userid}</td>
                                <td className={showPending ? "hover:bg-base-300" : ""}>{transaction.amount} kr</td>
                                <td className={showPending ? "hover:bg-base-300" : ""}>{formatDate(transaction.timestamp)}</td>
                            </tr>
                        ))
                    )}
                    </tbody>
                </table>
            </div>






            <dialog id="approve_transaction_modal" className="modal">
                <div className="modal-box">
                    <h3 className="font-bold text-lg mb-4">Approve Transaction</h3>
                    {selectedTransaction && (
                        <div className="space-y-4">
                            <div className="flex justify-between">
                                <span className="font-semibold">Transaction Number:</span>
                                <span>{selectedTransaction.transactionNumber || selectedTransaction.transactionnumber}</span>
                            </div>
                            <div className="flex justify-between">
                                <span className="font-semibold">User ID:</span>
                                <span>{selectedTransaction.userId || selectedTransaction.userid}</span>
                            </div>
                            <div className="flex justify-between">
                                <span className="font-semibold">Amount:</span>
                                <span>{selectedTransaction.amount} kr</span>
                            </div>
                            <div className="flex justify-between">
                                <span className="font-semibold">Timestamp:</span>
                                <span>{formatDate(selectedTransaction.timestamp)}</span>
                            </div>
                            <div className="divider"></div>
                            <p className="text-center text-sm text-gray-600">
                                Are you sure you want to approve this transaction?
                            </p>
                        </div>
                    )}
                    <div className="modal-action">
                        <button type="button" className="btn px-6 py-3" onClick={handleCancel}>Cancel</button>
                        <button
                            type="button"
                            className="btn bg-[#E50006FF] text-white hover:bg-[#AF0006FF] px-6 py-3"
                            onClick={handleApprove}>Approve</button>
                    </div>
                </div>
                <form method="dialog" className="modal-backdrop">
                    <button onClick={handleCancel}>close</button>
                </form>
            </dialog>
        </>
    );
}

export default AdminTransactions;