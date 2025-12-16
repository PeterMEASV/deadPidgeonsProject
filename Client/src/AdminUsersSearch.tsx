import { useNavigate } from "react-router";
import { useEffect, useState } from "react";
import { userClient } from "./baseUrl.ts";
import type { SetUserActiveDTO, SetUserAdminDTO, UpdateUserDTO, User } from "./generated-ts-client";

function AdminUsersSearch() {
  const navigate = useNavigate();
  const [searchQuery, setSearchQuery] = useState("");
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  //bare taget fra latest.tsx
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [formData, setFormData] = useState<UpdateUserDTO>({});
  const [flags, setFlags] = useState<{ isAdmin: boolean; isActive: boolean }>({
    isAdmin: false,
    isActive: true,
  });

   //bruger override for at fixe problemet med søgning
  const handleSearch = async (queryOverride?: string) => {
    const q = (queryOverride ?? searchQuery).trim();
    setError(null);

    if (!q) {
      setUsers([]);
      return;
    }

    setLoading(true);
    try {
      const result = await userClient.searchUserByPhoneNumber(q);
      setUsers(Array.isArray(result) ? result : []);
    } catch (e) {
      console.error("Search failed", e);
      setError("Search failed. Please try again.");
      setUsers([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const q = searchQuery.trim();


    //query mimnimum ændrer vi her
    if (q.length < 3) {
      setUsers([]);
      setError(null);
      return;
    }

    //debouncer, står i ms
    const timeoutId = window.setTimeout(() => {
      void handleSearch(q);
    }, 400);

    return () => {
      window.clearTimeout(timeoutId);
    };
  }, [searchQuery]);

  //mere lort nakket fra latest.tsx
  const handleUserClick = (user: User) => {
    setSelectedUser(user);
    setFormData({
      firstname: user.firstname,
      lastname: user.lastname,
      email: user.email,
      phonenumber: user.phonenumber,
      password: undefined,
    });

    setFlags({
      isAdmin: user.isadmin ?? false,
      isActive: user.isactive ?? true,
    });

    (document.getElementById("edit_user_modal") as HTMLDialogElement)?.showModal();
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => {
      if (name === "password") {
        return { ...prev, password: value.trim() === "" ? undefined : value };
      }
      return { ...prev, [name]: value };
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedUser?.id) return;

    try {
      let updatedUser = await userClient.updateUser(selectedUser.id, formData);

      const originalIsAdmin = selectedUser.isadmin ?? false;
      const originalIsActive = selectedUser.isactive ?? true;

      if (originalIsAdmin !== flags.isAdmin) {
        const dto: SetUserAdminDTO = { isAdmin: flags.isAdmin };
        updatedUser = await userClient.setUserAdminStatus(selectedUser.id, dto);
      }

      if (originalIsActive !== flags.isActive) {
        const dto: SetUserActiveDTO = { isActive: flags.isActive };
        updatedUser = await userClient.setUserActiveStatus(selectedUser.id, dto);
      }

      setUsers((prev) => prev.map((u) => (u.id === updatedUser.id ? updatedUser : u)));
      (document.getElementById("edit_user_modal") as HTMLDialogElement)?.close();
      setSelectedUser(null);
    } catch (err) {
      console.error("Failed to update user:", err);
    }
  };

  const handleCancel = () => {
    (document.getElementById("edit_user_modal") as HTMLDialogElement)?.close();
    setSelectedUser(null);
  };

  return (
    <>
      <div className="flex justify-center gap-3 mt-6 mb-4">
        <button className="btn bg-[#E50006FF] text-white text-xl px-8 py-4 h-auto hover:bg-[#AF0006FF]">Search</button>
        <button className="btn bg-[#E50006FF] text-white text-xl px-8 py-4 h-auto hover:bg-[#AF0006FF]" onClick={() => navigate("/admin/users/latest")}>Latest</button>
      </div>

      <div className="flex items-start justify-center min-h-[calc(100vh-300px)]">
          <div className="w-full max-w-2xl px-4">
            <input type="text" placeholder="Search by phone number..." className="input w-full text-lg py-6 h-auto border-4 border-[#a8a8a8] focus:outline-none rounded-xl" value={searchQuery} onChange={(e) => setSearchQuery(e.target.value)}
                   onKeyDown={(e) => e.key === "Enter" && void handleSearch()}/>

          <div className="mt-3">
              {loading && <div className="text-sm text-gray-500 px-2 py-2">Searching…</div>}

            {error && (
              <div className="alert alert-error">
                <span>{error}</span>
              </div>
            )}
              {/* ved ikke helt hvordan den her virker, men den er ansvarlig for at table kun kommer op når der søges på noget */}
            {searchQuery.trim() !== "" && (
              <div className="overflow-x-auto">
                <table className="table">
                  <thead>
                    <tr className="text-center bg-[#bfbfbd]">
                      <th>Name</th>
                      <th>Phone Number</th>
                      <th>E-Mail</th>
                    </tr>
                  </thead>
                  <tbody>
                    {users.length === 0 ? (
                      <tr>
                        <td colSpan={3} className="text-center py-8 text-gray-500">
                          No users found
                        </td>
                      </tr>
                    ) : (
                      users.map((u, index) => (
                        <tr key={u.id ?? `${u.email ?? "no-email"}-${u.phonenumber ?? "no-phone"}-${index}`}
                          className={`hover:bg-base-300 cursor-pointer text-center ${index % 2 !== 0 ? "bg-[#bfbfbd]" : ""}`}
                          onClick={() => handleUserClick(u)}>
                            <td>{u.firstname} {u.lastname}</td>
                          <td>{u.phonenumber}</td>
                            <td>{u.email}</td>
                        </tr>
                      )))}
                  </tbody>
                </table>
              </div>
            )}
          </div>
        </div>
      </div>





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
                  value={formData.firstname || ""}
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
                  value={formData.lastname || ""}
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
                  value={formData.email || ""}
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
                  value={formData.phonenumber || ""}
                  onChange={handleInputChange}
                  className="input input-bordered w-80"
                />
              </div>
            </div>

            <div className="form-control mb-4">
              <div className="flex justify-between items-center gap-8">
                <span className="label-text text-base">Admin</span>
                <div className="flex gap-6 w-80">
                  <label className="label cursor-pointer gap-3">
                    <input
                      type="radio"
                      name="isAdmin"
                      className="radio"
                      checked={flags.isAdmin === true}
                      onChange={() => setFlags((prev) => ({ ...prev, isAdmin: true }))}
                    />
                    <span className="label-text">Yes</span>
                  </label>

                  <label className="label cursor-pointer gap-3">
                    <input
                      type="radio"
                      name="isAdmin"
                      className="radio"
                      checked={flags.isAdmin === false}
                      onChange={() => setFlags((prev) => ({ ...prev, isAdmin: false }))}
                    />
                    <span className="label-text">No</span>
                  </label>
                </div>
              </div>
            </div>

            <div className="form-control mb-4">
              <div className="flex justify-between items-center gap-8">
                <span className="label-text text-base">Active</span>
                <div className="flex gap-6 w-80">
                  <label className="label cursor-pointer gap-3">
                    <input
                      type="radio"
                      name="isActive"
                      className="radio"
                      checked={flags.isActive === true}
                      onChange={() => setFlags((prev) => ({ ...prev, isActive: true }))}
                    />
                    <span className="label-text">Yes</span>
                  </label>

                  <label className="label cursor-pointer gap-3">
                    <input
                      type="radio"
                      name="isActive"
                      className="radio"
                      checked={flags.isActive === false}
                      onChange={() => setFlags((prev) => ({ ...prev, isActive: false }))}
                    />
                    <span className="label-text">No</span>
                  </label>
                </div>
              </div>
            </div>

            <div className="form-control mb-4">
              <div className="flex justify-between items-center gap-8">
                <span className="label-text text-base">Password</span>
                <input
                  type="password"
                  name="password"
                  value={formData.password || ""}
                  onChange={handleInputChange}
                  className="input input-bordered w-80"
                  placeholder="Password"
                  autoComplete="new-password"
                />
              </div>
            </div>

            <div className="modal-action">
              <button type="button" className="btn px-6 py-3" onClick={handleCancel}>
                Cancel
              </button>
              <button
                type="submit"
                className="btn bg-[#E50006FF] text-white hover:bg-[#AF0006FF] px-6 py-3"
              >
                Save
              </button>
            </div>
          </form>
        </div>

        <form method="dialog" className="modal-backdrop">
          <button onClick={handleCancel}>close</button>
        </form>
      </dialog>
    </>
  );
}

export default AdminUsersSearch;