import {Outlet, useNavigate} from 'react-router';
import logo from "./Assets/logo_png.png";
import { useSetAtom, useAtomValue } from 'jotai';

import { useAuth } from './Hooks/auth.tsx';
import {userInfoAtom} from "./Token.tsx";


function PlayerPage() {
    const navigate = useNavigate()
    const setUser = useSetAtom(userInfoAtom);
    const user = useAtomValue(userInfoAtom);
    const { logout } = useAuth();

    const handleLogout = () => {
        localStorage.removeItem('user');
        logout();
        setUser(null);
        navigate('/login');
    };

    const getUserName = () => {
        const first = user?.firstname || '';
        const last = user?.lastname || '';
        return `${first} ${last}`.trim() || 'User';
    };

    const getBalance = () => {
        return user?.balance ?? 0;
    };

    return (
        <>
            <div className="navbar bg-[#F44336] shadow-sm" >
                <div className="navbar-start">
                    <div className="cursor-pointer" onClick={() => navigate('/Player')}>
                        <img src={logo} alt="Jerne IF" className="h-24" />
                    </div>
                </div>
                <div className="flex-1">
                    <div className="flex justify-between w-full gap-20 px-8">
                        <button className="border-2 border-white text-white px-4 py-2 rounded-lg hover:bg-white hover:text-[#F44336] transition-colors whitespace-nowrap" onClick={() => navigate('/Player/game')}>
                            Nuværende Spil
                        </button>
                        <button className="border-2 border-white text-white px-4 py-2 rounded-lg hover:bg-white hover:text-[#F44336] transition-colors whitespace-nowrap" onClick={() => navigate('/Player/transactions')}>
                            Transaktioner
                        </button>
                        <button className="border-2 border-white text-white px-4 py-2 rounded-lg hover:bg-white hover:text-[#F44336] transition-colors whitespace-nowrap" onClick={() => navigate('/Player/history')}>
                            Historik
                        </button>
                    </div>
                </div>
                <div className="navbar-end -mt-8 flex items-center gap-4">
                    <span className="text-white font-semibold">{getBalance()} DKK</span>
                    <div className="dropdown dropdown-end">
                        <div tabIndex={0} role="button" className="btn btn-ghost text-white border-2 border-white hover:bg-white hover:text-[#F44336] transition-colors">
                            {getUserName()}
                        </div>
                        <ul
                            tabIndex={-1}
                            className="menu menu-sm dropdown-content bg-base-100 rounded-box z-1 mt-3 w-52 p-2 shadow">
                            <li><a onClick={handleLogout}>Logout</a></li>
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

export default PlayerPage;