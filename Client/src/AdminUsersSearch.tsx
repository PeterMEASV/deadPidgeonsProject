import {useNavigate} from "react-router";
import {useState} from "react";

function AdminUsersSearch() {
    const navigate = useNavigate();
    const [searchQuery, setSearchQuery] = useState("");

    const handleSearch = () => {
        console.log("Searching for:", searchQuery);
        // Add your search logic here
    };

    return (
        <>
            <div className="flex justify-center gap-3 mt-6 mb-4">
                <button className="btn bg-[#E50006FF] text-white text-xl px-8 py-4 h-auto hover:bg-[#AF0006FF]" >Search</button>
                <button className="btn bg-[#E50006FF] text-white text-xl px-8 py-4 h-auto hover:bg-[#AF0006FF]" onClick={() => navigate('/admin/users/latest')}>Latest</button>
            </div>

            <div className="flex items-center justify-center min-h-[calc(100vh-300px)]">
                <div className="w-full max-w-2xl px-4">
                    <input
                        type="text"
                        placeholder="Search users..."
                        className="input w-full text-lg py-6 h-auto border-4 border-[#a8a8a8] focus:outline-none rounded-xl"
                        value={searchQuery}
                        onChange={(e) => setSearchQuery(e.target.value)}
                        onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
                    />
                </div>
            </div>
        </>
    )
}
export default AdminUsersSearch