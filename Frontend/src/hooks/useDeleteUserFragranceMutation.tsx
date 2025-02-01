import { useMutation, useQueryClient } from "@tanstack/react-query";
import useUserController from "../api/controllers/useUserController";
import { useState } from "react";
import { ConflictError, NotFoundError } from "../dto-s/Errors";

const useDeleteUserFragranceMutation = () => {
  const { deleteFragrance } = useUserController();
  const [deleteFragranceError, setDeleteFragranceError] = useState<
    string | null
  >(null);
  const queryClient = useQueryClient();

  const deleteUserFragranceMutation = useMutation(deleteFragrance, {
    onSuccess: () => {
      setDeleteFragranceError(null);
      queryClient.refetchQueries();
    },
    onError: (error) => {
      if (error instanceof NotFoundError) {
        setDeleteFragranceError("Fragrance not found!");
      } else if (error instanceof ConflictError) {
        setDeleteFragranceError("You don't own this fragrance!");
      } else {
        setDeleteFragranceError("Error 500");
      }
    },
  });
  return { deleteUserFragranceMutation, deleteFragranceError };
};

export default useDeleteUserFragranceMutation;
