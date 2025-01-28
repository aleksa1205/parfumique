import React from "react";
import FragranceCard from "./FragranceCard";
import DeleteButton from "./UiComponents/DeleteButton";
import useDeleteUserFragranceMutation from "../hooks/useDeleteUserFragranceMutation";
import { FragranceCardProps } from "../dto-s/Props";
import { CircleLoader } from "./loaders/CircleLoader";

const FragranceCardProfile: React.FC<FragranceCardProps> = (props) => {
  const { deleteUserFragranceMutation, deleteFragranceError } =
    useDeleteUserFragranceMutation();

  const onSubmit = async (id: number) => {
    await deleteUserFragranceMutation.mutateAsync(id);
  };

  if (deleteUserFragranceMutation.isLoading) {
    return <CircleLoader />;
  }

  return (
    <div className="relative grid gap-4 w-full">
      <FragranceCard {...props} />
      <DeleteButton onClick={() => onSubmit(props.id)} />
      {deleteFragranceError && (
        <div className="error text-center"> {deleteFragranceError}</div>
      )}
    </div>
  );
};

export default FragranceCardProfile;
