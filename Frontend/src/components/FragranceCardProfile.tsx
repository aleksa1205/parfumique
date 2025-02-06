import useDeleteUserFragranceMutation from "../hooks/useDeleteUserFragranceMutation";
import { BaseFragrance } from "../dto-s/FragranceDto";
import { CircleLoader } from "./loaders/CircleLoader";
import FragranceCard from "./FragranceCard";
import DeleteButton from "./UiComponents/Buttons/DeleteButton";

const FragranceCardProfile = (props: BaseFragrance) => {
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