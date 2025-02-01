import UseAuth from "./useAuth";
import { useLocation, useNavigate } from "react-router-dom";
import { emptyAuthValues } from "../context/AuthProvider";

function useLogout() {
  const { setAuth } = UseAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const logoutUser = async function (message?: string) {
    try {
      let urlMessage = "";
      if (message)
        urlMessage = "?message=" + message;

      setAuth(emptyAuthValues);
      navigate("/login" + urlMessage, { state: {from: location, replace: true}});
    } catch (error) {
      console.log(error);
    }
  };
  
  return logoutUser;
};

export default useLogout;
