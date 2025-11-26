import {Outlet, useNavigate} from 'react-router';
import logo from './Assets/logo_png.png';

function AdminPage() {
    const navigate = useNavigate()

    return (
        <>
            <div className="navbar bg-[#F44336] shadow-sm" >
                <div className="navbar-start">
                    <div className="cursor-pointer" onClick={() => navigate('/Admin')}>
                        <img src={logo} alt="Jerne IF" className="h-24" />
                    </div>
                </div>
                <div className="flex-1">
                    <div className="flex justify-between w-full gap-20 px-8">
                        <button className="border-2 border-white text-white px-4 py-2 rounded-lg hover:bg-white hover:text-[#F44336] transition-colors whitespace-nowrap" onClick={() => navigate('/admin/button1')}>
                            Nuværende Spil
                        </button>
                        <button className="border-2 border-white text-white px-4 py-2 rounded-lg hover:bg-white hover:text-[#F44336] transition-colors whitespace-nowrap" onClick={() => navigate('/admin/button2')}>
                            Brugere
                        </button>
                        <button className="border-2 border-white text-white px-4 py-2 rounded-lg hover:bg-white hover:text-[#F44336] transition-colors whitespace-nowrap" onClick={() => navigate('/admin/button3')}>
                            Transaktioner
                        </button>
                        <button className="border-2 border-white text-white px-4 py-2 rounded-lg hover:bg-white hover:text-[#F44336] transition-colors whitespace-nowrap" onClick={() => navigate('/admin/button4')}>
                            Historik
                        </button>
                    </div>
                </div>
                <div className="navbar-end -mt-8">
                    <div className="dropdown dropdown-end">
                        <div tabIndex={0} role="button" className="btn btn-ghost btn-circle avatar">
                            <div className="w-10 rounded-full">
                                <img
                                    alt="Tailwind CSS Navbar component"
                                    src="https://img.daisyui.com/images/stock/photo-1534528741775-53994a69daeb.webp" />
                            </div>
                        </div>
                        <ul
                            tabIndex={-1}
                            className="menu menu-sm dropdown-content bg-base-100 rounded-box z-1 mt-3 w-52 p-2 shadow">
                            <li>
                                <a className="justify-between">
                                    Profile
                                    <span className="badge">New</span>
                                </a>
                            </li>
                            <li><a>Settings</a></li>
                            <li><a>Logout</a></li>
                        </ul>
                    </div>
                </div>
                </div>
            <main className="mainContent">
                <Outlet />
            </main>
        </>
    );
}

export default AdminPage;