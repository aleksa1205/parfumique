import UseAuth from "./useAuth";
import { useNavigate } from "react-router-dom";
import { emptyAuthValues } from "../context/AuthProvider";

const useLogout = () => {
  const { setAuth } = UseAuth();
  const navigate = useNavigate();

  const logoutUser = async function () {
    try {
      setAuth(emptyAuthValues);
      localStorage.clear();
      navigate("/login");
    } catch (error) {
      console.log(error);
    }
  };
  return logoutUser;
};

export default useLogout;
