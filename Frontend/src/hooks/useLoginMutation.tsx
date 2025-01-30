import { useState } from "react";
import useUserController from "../api/controllers/useUserController";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { WrongCredentials } from "../dto-s/Errors";
import UseAuth from "./useAuth";

const useLoginMutation = () => {
  const { login } = useUserController();
  const { setAuth } = UseAuth();
  const [credentialError, setCredentialError] = useState<string | null>(null);
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const loginMutation = useMutation(login, {
    onSuccess: (response) => {
      setAuth({ jwtToken: response.token, username: response.username, role: response.role });
      setCredentialError(null);
      queryClient.invalidateQueries();
      navigate("/user-profile");
    },
    onError: (error) => {
      if (error instanceof WrongCredentials) {
        setCredentialError("Invalid username or password!");
      } else {
        setCredentialError("Error 500");
      }
    },
  });
  return { loginMutation, credentialError };
};

export default useLoginMutation;
