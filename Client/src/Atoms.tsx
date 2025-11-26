import {atom} from "jotai";
import type {RouteObject} from "react-router";
import Topbar from "./Topbar.tsx";
import Home from "./Home.tsx";
import TemplatePage from "./TemplatePage.tsx";

export const routesAtom = atom<RouteObject[]>([
    {
        path: '/',
        element: <Topbar />,
        children: [
            {
                index: true,
                element: <Home />
            },
            {
                path: 'template',
                element: <TemplatePage />
            }
        ]
    }
])