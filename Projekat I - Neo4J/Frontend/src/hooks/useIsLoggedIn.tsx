import { useEffect, useState } from "react";
import UseAuth from "./useAuth";

const useIsLoggedIn = () => {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const { auth } = UseAuth();
  useEffect(() => {
    if (auth.jwtToken.trim()) {
      setIsLoggedIn(true);
    } else {
      setIsLoggedIn(false);
    }
  }, [auth]);

  return isLoggedIn;
};

export default useIsLoggedIn;
