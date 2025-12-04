import { useState } from 'react';
import { useNavigate } from 'react-router';
import { AuthClient } from './generated-ts-client';
import { finalUrl } from './baseUrl';
import { useSetAtom } from 'jotai';
import { userAtom } from './Atoms';

export function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const setUser = useSetAtom(userAtom);

    const authClient = new AuthClient(finalUrl);

    const handleLogin = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        try {
            const response = await authClient.login({
                email,
                password,
            });

            // Store user info
            setUser(response);
            localStorage.setItem('user', JSON.stringify(response));

            // Redirect based on role
            if (response.isadmin) {
                navigate('/admin/game');
            } else {
                navigate('/player/game');
            }
        } catch (err: any) {
            console.error('Login error:', err);
            setError('Invalid email or password, or account is inactive');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-base-200">
            <div className="card w-96 bg-base-100 shadow-xl">
                <div className="card-body">
                    <h2 className="card-title text-center text-2xl mb-4">🐦 Dead Pigeons Login</h2>

                    <form onSubmit={handleLogin}>
                        <div className="form-control">
                            <label className="label">
                                <span className="label-text">Email</span>
                            </label>
                            <input
                                type="email"
                                placeholder="email@example.com"
                                className="input input-bordered"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                required
                            />
                        </div>

                        <div className="form-control mt-4">
                            <label className="label">
                                <span className="label-text">Password</span>
                            </label>
                            <input
                                type="password"
                                placeholder="••••••••"
                                className="input input-bordered"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                required
                            />
                        </div>

                        {error && (
                            <div className="alert alert-error mt-4">
                                <span>{error}</span>
                            </div>
                        )}

                        <div className="form-control mt-6">
                            <button
                                type="submit"
                                className="btn btn-primary"
                                disabled={loading}
                            >
                                {loading ? (
                                    <span className="loading loading-spinner"></span>
                                ) : (
                                    'Login'
                                )}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
}
