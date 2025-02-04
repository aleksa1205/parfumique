import { createContext } from "react";
import { GetUserResponse } from "../dto-s/UserDto";
import { useQuery } from "@tanstack/react-query";
import useUserController from "../api/controllers/useUserController";
import UseAuth from "../hooks/useAuth";

type CurrUserContextType = {
  user?: GetUserResponse;
  isLoading: boolean;
  refetch:() => void;
};

export const emptyUserContext: CurrUserContextType = {
  user: undefined,
  isLoading: true,
  refetch: () => {}
};

export const CurrUserContext =
  createContext<CurrUserContextType>(emptyUserContext);

type PropsValue = {
  children: React.ReactNode;
};

export function CurrUserProvider({ children }: PropsValue) {
  const { get } = useUserController();
  const { auth } = UseAuth();

  const { data: userData, isLoading, refetch } = useQuery(
    ["user", auth.username],
    () => get(),
    {
      enabled: !!auth.jwtToken,
      retry: false,
      onError: (error) => {
        console.log("Error while fetching user data: ", error);
      },
    }
  );
  const contextValue: CurrUserContextType = {
    user: userData,
    isLoading: isLoading,
    refetch
  };
  return (
    <CurrUserContext.Provider value={contextValue}>
      {children}
    </CurrUserContext.Provider>
  );
}
