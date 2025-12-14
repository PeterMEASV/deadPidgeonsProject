import {atom} from "jotai";
import type {RouteObject} from "react-router";
import Home from "./Home.tsx";
import AdminPage from "./AdminPage.tsx";
import PlayerPage from "./PlayerPage.tsx";
import AdminGame from "./AdminGame.tsx";
import PlayerGame from "./PlayerGame.tsx";
import AdminUsersSearch from "./AdminUsersSearch.tsx";
import AdminTransactions from "./AdminTransactions.tsx";
import AdminHistory from "./AdminHistory.tsx";
import AdminUsersLatest from "./AdminUsersLatest.tsx";
import PlayerTransactions from "./PlayerTransactions.tsx";
import PlayerHistory from "./PlayerHistory.tsx";
import PlayerNewGame from "./PlayerNewGame.tsx";
import {Login} from "./Login.tsx";
import type {LoginResponseDTO} from "./generated-ts-client";

export const userAtom = atom<LoginResponseDTO | null>(null);
export const routesAtom = atom<RouteObject[]>([
    {
        path: '/',
        element: <Home />
    },
    {
        path: 'login',
        element: <Login />
    },
    {
        path: '/Admin',
        element: <AdminPage />,
        children: [
            {
                path: 'Game',
                element: <AdminGame />
            },
            {
                path: 'Users',
                children: [
                    {
                        path: 'Search',
                        element: <AdminUsersSearch />
                    },
                    {
                        path: 'Latest',
                        element: <AdminUsersLatest />
                    }
                ]
            },
            {
                path: 'Transactions',
                element: <AdminTransactions />
            },
            {
                path: 'History',
                element: <AdminHistory />
            }
        ]
    },
    {
        path: '/Player',
        element: <PlayerPage />,
        children: [
            {
                path: 'Game',
                element: <PlayerGame />
            },
            {
                path: 'Transactions',
                element: <PlayerTransactions />
            },
            {
                path: 'History',
                element: <PlayerHistory />
            },
            {
                path: 'NewGame',
                element: <PlayerNewGame />
            }
            
        ]
    }
])