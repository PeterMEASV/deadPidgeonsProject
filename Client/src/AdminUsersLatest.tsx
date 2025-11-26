import {useNavigate} from "react-router";

function AdminUsersLatest() {
    const navigate = useNavigate();


    return (
        <>
            <div className="flex justify-center gap-3 mt-6 mb-4">
                <button className="btn bg-[#E50006FF] text-white text-xl px-8 py-4 h-auto hover:bg-[#AF0006FF]" onClick={() => navigate('/admin/users/search')}>Search</button>
                <button className="btn bg-[#E50006FF] text-white text-xl px-8 py-4 h-auto hover:bg-[#AF0006FF]">Latest</button>
            </div>

            <div className="overflow-x-auto ">
                <table className="table">
                    {/* head */}
                    <thead>
                    <tr className="text-center bg-base-200">
                        <th>Name</th>
                        <th>Phone Number</th>
                        <th>E-Mail</th>
                    </tr>
                    </thead>
                    <tbody>
                    <tr className="hover:bg-base-300 cursor-pointer text-center">
                        <td>Cy Ganderton</td>
                        <td>+4512345678</td>
                        <td>BananaMaster@gmail.com</td>
                    </tr>

                    <tr className="hover:bg-base-300 cursor-pointer text-center bg-base-200">
                        <td>Cy Ganderton</td>
                        <td>+4512345678</td>
                        <td>BananaMaster@gmail.com</td>
                    </tr>

                    <tr className="hover:bg-base-300 cursor-pointer text-center">
                        <td>Cy Ganderton</td>
                        <td>+4512345678</td>
                        <td>BananaMaster@gmail.com</td>
                    </tr>

                    <tr className="hover:bg-base-300 cursor-pointer text-center bg-base-200">
                        <td>Cy Ganderton</td>
                        <td>+4512345678</td>
                        <td>BananaMaster@gmail.com</td>
                    </tr>

                    <tr className="hover:bg-base-300 cursor-pointer text-center">
                        <td>Cy Ganderton</td>
                        <td>+4512345678</td>
                        <td>BananaMaster@gmail.com</td>
                    </tr>

                    <tr className="hover:bg-base-300 cursor-pointer text-center bg-base-200">
                        <td>Cy Ganderton</td>
                        <td>+4512345678</td>
                        <td>BananaMaster@gmail.com</td>
                    </tr>

                    </tbody>
                </table>
            </div>
        </>
    )
}
export default AdminUsersLatest