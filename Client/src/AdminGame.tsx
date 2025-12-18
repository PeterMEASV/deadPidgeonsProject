import { useState } from 'react';
import { gameClient} from "./baseUrl.ts";

function AdminGame() {
    const [toggledButtons, setToggledButtons] = useState<Set<number>>(new Set());
    const [showConfirmation, setShowConfirmation] = useState(false);

    const handleButtonClick = (index: number) => {
        setToggledButtons(prev => {
            const newSet = new Set(prev);
            if (newSet.has(index)) {
                newSet.delete(index);
            } else {
                newSet.add(index);
            }
            return newSet;
        });
        console.log(`Button ${index + 1} clicked`);
    };

    const handleEndGame = () => {
        setShowConfirmation(true);
    };

    const handleConfirm = async () => {
        console.log('Game ended');
        setShowConfirmation(false);
        const toggledNumbers = Array.from(toggledButtons).map((i) => i + 1);
        console.log(gameClient.createGame({ winningNumbers: toggledNumbers }));
    };

    const handleCancel = () => {
        setShowConfirmation(false);
    };

    return (
        <>
            <div className="grid grid-cols-4 gap-4 w-full p-4">
                {Array.from({ length: 16 }).map((_, index) => (
                    <button key={index} className={`btn h-32 text-3xl flex items-center justify-center ${toggledButtons.has(index)
                                ? 'bg-[#E50006FF] text-white hover:bg-[#AF0006FF]'
                                : 'bg-[#F44336] text-white hover:bg-[#c93232]'}`}
                        onClick={() => handleButtonClick(index)}>{toggledButtons.has(index) ? (<div className="w-16 h-16 rounded-full bg-white flex items-center justify-center text-[#E50006FF] font-bold">{index + 1}</div>) : (index + 1)}
                    </button>
                ))}
            </div>
            <div className="flex justify-center mt-6 mb-4">
                <button className="btn bg-[#E50006FF] text-white text-xl px-8 py-4 h-auto hover:bg-[#AF0006FF]" onClick={handleEndGame}>Afslut Spil</button>
            </div>

            {showConfirmation && (
                <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
                    <div className="bg-white rounded-lg p-6 max-w-md w-full mx-4">
                        <h2 className="text-xl font-bold mb-4">Er du sikker?</h2>
                        <p>Vil du afslutte spillet?</p>
                        <div className="flex justify-end gap-4">
                            <button className="btn btn-ghost px-6 py-3" onClick={handleCancel}>Annuller</button>
                            <button className="btn bg-[#E50006FF] text-white hover:bg-[#AF0006FF] px-6 py-3" onClick={handleConfirm}>Bekræft</button>
                        </div>
                    </div>
                </div>
            )}
        </>
    )
}
export default AdminGame