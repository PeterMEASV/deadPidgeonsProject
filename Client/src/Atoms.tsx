import { atom } from "jotai";
import type { RouteObject } from "react-router";
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
import { Login } from "./Login.tsx";
import type { LoginResponseDTO } from "./generated-ts-client";
import RequireAuth from "./RequireAuth";

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
    element: <RequireAuth />,
    children: [
      {
        path: '/admin',
        element: <AdminPage />,
        children: [
          { path: 'game', element: <AdminGame /> },
          {
            path: 'users',
            children: [
              { path: 'search',
                element: <AdminUsersSearch />
              },

              { path: 'latest',
                element: <AdminUsersLatest />
              },

            ],
          },
          { path: 'transactions',
            element: <AdminTransactions />
          },

          { path: 'history',
            element: <AdminHistory />
          },
        ],
      },
      {
        path: '/player',
        element: <PlayerPage />,
        children: [
          { path: 'game',
            element: <PlayerGame />
          },

          { path: 'transactions',
            element: <PlayerTransactions />
          },

          { path: 'history',
            element: <PlayerHistory />
          },

          { path: 'newgame',
            element: <PlayerNewGame />
          },

        ],
      },
    ],
  },
]);