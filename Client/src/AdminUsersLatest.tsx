import {useNavigate} from "react-router";
import {useEffect, useState} from "react";
import {userClient} from "./baseUrl.ts";
import type {User} from "./generated-ts-client.ts";

function AdminUsersLatest() {
    const navigate = useNavigate();
    const [users, setUsers] = useState<User[]>([]);

    useEffect(() => {
        userClient.getAllUsers().then(r => {
            setUsers(r);
        })
    }, [])

    return (
        <>
            <div className="flex justify-center gap-3 mt-6 mb-4">
                <button className="btn bg-[#E50006FF] text-white text-xl px-8 py-4 h-auto hover:bg-[#AF0006FF]" onClick={() => navigate('/admin/users/search')}>Search</button>
                <button className="btn bg-[#E50006FF] text-white text-xl px-8 py-4 h-auto hover:bg-[#AF0006FF]">Latest</button>
            </div>

            <div className="overflow-x-auto ">
                <table className="table">
                    <thead>
                    <tr className="text-center bg-base-200">
                        <th>Name</th>
                        <th>Phone Number</th>
                        <th>E-Mail</th>
                    </tr>
                    </thead>
                    <tbody>
                    {users.map((user, index) => (
                        <tr 
                            key={user.id} 
                            className={`hover:bg-base-300 cursor-pointer text-center ${index % 2 !== 0 ? 'bg-base-200' : ''}`}>
                            <td>{user.firstname} {user.lastname}</td>
                            <td>{user.phonenumber}</td>
                            <td>{user.email}</td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            </div>
        </>
    )
}
export default AdminUsersLatest