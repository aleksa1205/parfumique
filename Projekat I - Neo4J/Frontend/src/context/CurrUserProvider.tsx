import { createContext } from "react";
import { GetUserResponse } from "../dto-s/responseTypes";
import { useQuery } from "@tanstack/react-query";
import useUserController from "../api/controllers/useUserController";
import UseAuth from "../hooks/useAuth";

type CurrUserContextType = {
  user?: GetUserResponse;
  isLoading: boolean;
};

export const emptyUserContext: CurrUserContextType = {
  user: undefined,
  isLoading: true,
};

export const CurrUserContext =
  createContext<CurrUserContextType>(emptyUserContext);

type PropsValue = {
  children: React.ReactNode;
};

export function CurrUserProvider({ children }: PropsValue) {
  const { get } = useUserController();
  const { auth } = UseAuth();
  const username = auth?.username;
  if (!username) {
    console.log("Username not found in auth!");
  }
  const { data: userData, isLoading } = useQuery(
    ["user", username],
    () => get(username),
    {
      enabled: !!username,
      retry: false,
      onError: (error) => {
        console.log("Error while fetching user data: ", error);
      },
    }
  );
  const contextValue: CurrUserContextType = {
    user: userData,
    isLoading: isLoading,
  };
  return (
    <CurrUserContext.Provider value={contextValue}>
      {children}
    </CurrUserContext.Provider>
  );
}
