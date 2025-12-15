import {useNavigate} from "react-router";
import {useEffect, useState} from "react";
import {userClient} from "./baseUrl.ts";
import type {User, UpdateUserDTO} from "./generated-ts-client.ts";

function AdminUsersLatest() {
    const navigate = useNavigate();
    const [users, setUsers] = useState<User[]>([]);
    const [selectedUser, setSelectedUser] = useState<User | null>(null);
    const [formData, setFormData] = useState<UpdateUserDTO>({});

    useEffect(() => {
        userClient.getAllUsers().then(r => {
            setUsers(r);
        })
    }, [])

    const handleUserClick = (user: User) => {
        setSelectedUser(user);
        setFormData({
            firstname: user.firstname,
            lastname: user.lastname,
            email: user.email,
            phonenumber: user.phonenumber,
            password: undefined,
        });
        (document.getElementById('edit_user_modal') as HTMLDialogElement)?.showModal();
    };

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;

        setFormData(prev => {

            //kun tilføjer password hvis den er tom
            if (name === "password") {
                return {
                    ...prev,
                    password: value.trim() === "" ? undefined : value,
                };
            }

            return {
                ...prev,
                [name]: value
            };
        });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!selectedUser?.id) return;

        try {
            const updatedUser = await userClient.updateUser(selectedUser.id, formData);
            setUsers(users.map(u => u.id === updatedUser.id ? updatedUser : u));
            (document.getElementById('edit_user_modal') as HTMLDialogElement)?.close();
        } catch (error) {
            console.error('Failed to update user:', error);
        }
    };

    const handleCancel = () => {
        (document.getElementById('edit_user_modal') as HTMLDialogElement)?.close();
        setSelectedUser(null);
    };

    return (
        <>
            <div className="flex justify-center gap-3 mt-6 mb-4">
                <button className="btn bg-[#E50006FF] text-white text-xl px-8 py-4 h-auto hover:bg-[#AF0006FF]" onClick={() => navigate('/admin/users/search')}>Search</button>
                <button className="btn bg-[#E50006FF] text-white text-xl px-8 py-4 h-auto hover:bg-[#AF0006FF]">Latest</button>
            </div>

            <div className="overflow-x-auto ">
                <table className="table">
                    <thead>
                    <tr className="text-center bg-[#bfbfbd]">
                        <th>Name</th>
                        <th>Phone Number</th>
                        <th>E-Mail</th>
                    </tr>
                    </thead>
                    <tbody>
                    {users.map((user, index) => (
                        <tr
                            key={user.id}
                            className={`hover:bg-base-300 cursor-pointer text-center ${index % 2 !== 0 ? 'bg-[#bfbfbd]' : ''}`}
                            onClick={() => handleUserClick(user)}>
                            <td>{user.firstname} {user.lastname}</td>
                            <td>{user.phonenumber}</td>
                            <td>{user.email}</td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            </div>

            {/* Edit User Modal */}
            <dialog id="edit_user_modal" className="modal">
                <div className="modal-box max-w-2xl">
                    <h3 className="font-bold text-lg mb-4">Edit User</h3>
                    <form onSubmit={handleSubmit}>
                        <div className="form-control mb-4">
                            <div className="flex justify-between items-center gap-8">
                                <span className="label-text text-base">First Name</span>
                                <input
                                    type="text"
                                    name="firstname"
                                    value={formData.firstname || ''}
                                    onChange={handleInputChange}
                                    className="input input-bordered w-80"
                                />
                            </div>
                        </div>

                        <div className="form-control mb-4">
                            <div className="flex justify-between items-center gap-8">
                                <span className="label-text text-base">Last Name</span>
                                <input
                                    type="text"
                                    name="lastname"
                                    value={formData.lastname || ''}
                                    onChange={handleInputChange}
                                    className="input input-bordered w-80"
                                />
                            </div>
                        </div>

                        <div className="form-control mb-4">
                            <div className="flex justify-between items-center gap-8">
                                <span className="label-text text-base">Email</span>
                                <input
                                    type="email"
                                    name="email"
                                    value={formData.email || ''}
                                    onChange={handleInputChange}
                                    className="input input-bordered w-80"
                                />
                            </div>
                        </div>

                        <div className="form-control mb-4">
                            <div className="flex justify-between items-center gap-8">
                                <span className="label-text text-base">Phone Number</span>
                                <input
                                    type="text"
                                    name="phonenumber"
                                    value={formData.phonenumber || ''}
                                    onChange={handleInputChange}
                                    className="input input-bordered w-80"
                                />
                            </div>
                        </div>

                        <div className="form-control mb-4">
                            <div className="flex justify-between items-center gap-8">
                                <span className="label-text text-base">Password</span>
                                <input
                                    type="password"
                                    name="password"
                                    value={formData.password || ''}
                                    onChange={handleInputChange}
                                    className="input input-bordered w-80"
                                    placeholder="Password"
                                    autoComplete="new-password"
                                />
                            </div>
                        </div>

                        <div className="modal-action">
                            <button type="button" className="btn px-6 py-3" onClick={handleCancel}>Cancel</button>
                            <button type="submit" className="btn bg-[#E50006FF] text-white hover:bg-[#AF0006FF] px-6 py-3">Save</button>
                        </div>
                    </form>
                </div>
                <form method="dialog" className="modal-backdrop">
                    <button onClick={handleCancel}>close</button>
                </form>
            </dialog>
        </>
    )
}
export default AdminUsersLatest;