import { useEffect } from 'react';
import { useNavigate } from 'react-router';
import { useAtomValue } from 'jotai';
import { userAtom } from './Atoms';

function Home() {
    const navigate = useNavigate();
    const user = useAtomValue(userAtom);

    useEffect(() => {
        // Load user from localStorage
        const storedUser = localStorage.getItem('user');

        if (storedUser) {
            const parsedUser = JSON.parse(storedUser);
            if (parsedUser.isAdmin) {
                navigate('/admin/game');
            } else {
                navigate('/player/game');
            }
        } else if (!user) {
            navigate('/login');
        }
    }, [navigate, user]);

    return (
        <div className="flex items-center justify-center min-h-screen">
            <span className="loading loading-spinner loading-lg"></span>
        </div>
    );
}

export default Home;
