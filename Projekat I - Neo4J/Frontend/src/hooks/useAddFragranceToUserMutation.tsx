import { useState } from "react";
import useUserController from "../api/controllers/useUserController";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { ConflictError, NotFoundError } from "../dto-s/Errors";

const useAddFragranceToUserMutation = () => {
  const { addFragrance } = useUserController();
  const [addFragranceError, setAddFragranceError] = useState<string | null>(
    null
  );
  const queryClient = useQueryClient();

  const addFragranceToUserMutation = useMutation(addFragrance, {
    onSuccess: () => {
      setAddFragranceError(null);
      queryClient.invalidateQueries();
    },
    onError: (error) => {
      if (error instanceof NotFoundError) {
        setAddFragranceError("Fragrance not found!");
      } else if (error instanceof ConflictError) {
        setAddFragranceError("You already own this fragrance!");
      } else {
        setAddFragranceError("Error 500");
      }
    },
  });
  return { addFragranceToUserMutation, addFragranceError };
};

export default useAddFragranceToUserMutation;
