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
    onSuccess: (_, id) => {
      setDeleteFragranceError(null);
      const storedFragrances = localStorage.getItem("selectedFragrances");
      const selectedFragrances = storedFragrances
        ? JSON.parse(storedFragrances) as Array<string>
        : [];

      if (selectedFragrances) {
        let fragIndex = -1;
        selectedFragrances.forEach((element, index) => {
          if (Number(element) == id) {
            fragIndex = index;
            return;
          }
        })

        if (fragIndex !== -1) {
          selectedFragrances.splice(fragIndex, 1);
          localStorage.setItem("selectedFragrances", JSON.stringify(selectedFragrances))
        }
      }

      
        
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
