import { createContext, useEffect, useState } from "react";

type ContextValues = {
  auth: AuthValues;
  setAuth: React.Dispatch<React.SetStateAction<AuthValues>>;
};

export type AuthValues = {
  jwtToken: string;
  username: string;
};

export const emptyAuthValues: AuthValues = {
  jwtToken: "",
  username: "",
};

type PropsValue = {
  children: React.ReactNode;
};

export const AuthContext = createContext<ContextValues>({
  auth: emptyAuthValues,
  setAuth: () => {},
});

const AuthProvider = ({ children }: PropsValue) => {
  const localStorageItem = localStorage.getItem("_auth");
  let defaultValues: AuthValues = emptyAuthValues;
  if (localStorageItem) {
    defaultValues = JSON.parse(localStorageItem);
  } else localStorage.removeItem("_auth");
  const [auth, setAuth] = useState<AuthValues>(defaultValues);
  useEffect(() => {
    localStorage.setItem("_auth", JSON.stringify(auth));
  }, [auth]);
  return (
    <AuthContext.Provider value={{ auth, setAuth }}>
      {children}
    </AuthContext.Provider>
  );
};

export default AuthProvider;
