import {atom} from "jotai";
import type {RouteObject} from "react-router";
import Home from "./Home.tsx";
import AdminPage from "./AdminPage.tsx";
import PlayerPage from "./PlayerPage.tsx";
import AdminGame from "./AdminGame.tsx";
import PlayerGame from "./PlayerGame.tsx";

export const routesAtom = atom<RouteObject[]>([
    {
        path: '/',
        element: <Home />
    },
    {
        path: '/Admin',
        element: <AdminPage />,
        children: [
            {
                path: 'Game',
                element: <AdminGame />
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
            }
        ]
    }
])