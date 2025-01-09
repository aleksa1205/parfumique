import { axiosAuth } from "../api/axios";
import { useEffect } from "react";
import UseAuth from "./useAuth";
import { emptyAuthValues } from "../context/AuthProvider";
// Ovo je react hook, kada se pozove on procita context iz auth i na osnovu njega
// konfigurise axiosAuth instancu
function useAxiosAuth() {
  const { auth, setAuth } = UseAuth();

  //const logout = useLogout2();

  useEffect(() => {
    // Inicijalni request, on treba da jwt (koji se nalazi u authContext) upise u secure
    // cookie i tako ga posalje serveru, jer server jedino tako proverava tokene
    const requestIntercept = axiosAuth.interceptors.request.use(
      (config) => {
        if (!config.headers["Authorization"]) {
          config.headers["Authorization"] = `Bearer ${auth.jwtToken}`;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );

    // Intrceptor za refreshovanje tokena, znaci ako request ne uspe i vrati nam nazad
    // 401 (Unauthorized) onda pozivamo refresh funkciju koja zove refresh endpoint
    // E sad verovatno moze da dodje do problema jer 401 kod moze da se vrati ako
    // uopste nemamo token
    const responseIntercept = axiosAuth.interceptors.response.use(
      (response) => response,
      async (error) => {
        const prevRequest = error?.config;
        if (error?.response?.status === 401 && !prevRequest?.sent) {
          prevRequest.sent = true;
          try {
            setAuth(emptyAuthValues);
            //redirect na login
          } catch (refreshError) {
            return Promise.reject(refreshError);
          }
        }

        return Promise.reject(error);
      }
    );

    //cleanup za useEffect
    return () => {
      axiosAuth.interceptors.request.eject(requestIntercept);
      axiosAuth.interceptors.response.eject(responseIntercept);
    };
  }, [auth]);

  return axiosAuth;
}

export default useAxiosAuth;
