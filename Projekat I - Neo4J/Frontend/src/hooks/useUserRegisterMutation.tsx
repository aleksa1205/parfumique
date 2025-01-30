import { useState } from "react";
import useUserController from "../api/controllers/useUserController";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { UsernameExists } from "../dto-s/Errors";
import { useNavigate } from "react-router-dom";

const useUserRegisterMutation = () => {
  const { register } = useUserController();
  const [registerError, setRegisterError] = useState<string | null>(null);
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const registerMutation = useMutation(register, {
    onSuccess: () => {
      setRegisterError(null);
      queryClient.invalidateQueries();
      navigate("/login");
    },
    onError: (error) => {
      if (error instanceof UsernameExists) {
        setRegisterError(
          "Username already exists. Please choose a different username."
        );
      } else {
        setRegisterError("An unexpected error occurred. Please try again.");
      }
    },
  });
  return { registerMutation, registerError };
};

export default useUserRegisterMutation;
