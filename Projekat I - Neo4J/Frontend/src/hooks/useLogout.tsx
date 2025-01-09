import UseAuth from "./useAuth";
import { useNavigate } from "react-router-dom";
import { emptyAuthValues } from "../context/AuthProvider";

const useLogout = () => {
  const { setAuth } = UseAuth();
  const navigate = useNavigate();

  const LogoutUser = async function () {
    try {
      setAuth(emptyAuthValues);
      setTimeout(() => {
        navigate("/login");
      }, 100);
    } catch (error) {
      console.log(error);
    }
  };
  return LogoutUser;
};

export default useLogout;
