import {useAtomValue, useSetAtom} from "jotai";
import {routesAtom} from "./Atoms.tsx";
import {useEffect, useMemo} from "react";
import {createBrowserRouter, RouterProvider} from "react-router";
import { userInfoAtom, userInfoQueryAtom } from "./Token.tsx";

function App() {
  const routes = useAtomValue(routesAtom);
  const router = useMemo(() => createBrowserRouter(routes), [routes]);

  const setUserInfo = useSetAtom(userInfoAtom);
  const userInfo = useAtomValue(userInfoQueryAtom);

  useEffect(() => {
    // userInfoQueryAtom resolves to User|null; push into writable cache
    setUserInfo(userInfo);
  }, [userInfo, setUserInfo]);

  return <RouterProvider router={router} />;
}

export default App;
