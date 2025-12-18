import { useEffect } from "react";
import { useNavigate } from "react-router";
import { useAtomValue } from "jotai";
import { userInfoAtom } from "./Token";

function Home() {
  const navigate = useNavigate();
  const userInfo = useAtomValue(userInfoAtom);

  useEffect(() => {
    if (!userInfo) {
      void navigate("/login");
      return;
    }

    if (userInfo.isadmin) {
      void navigate("/Admin/Game");
    } else {
      void navigate("/Player/Game");
    }
  }, [navigate, userInfo]);

  return (
    <div className="flex items-center justify-center min-h-screen">
      <span className="loading loading-spinner loading-lg"></span>
    </div>
  );
}

export default Home;
