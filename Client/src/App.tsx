import {useAtomValue} from "jotai";
import {routesAtom} from "./Atoms.tsx";
import {useMemo} from "react";
import {createBrowserRouter, RouterProvider} from "react-router";

function App() {
    const routes = useAtomValue(routesAtom);
    const router = useMemo(() => createBrowserRouter(routes), [routes]);
    return <><RouterProvider router = {router}/></>
}

export default App
