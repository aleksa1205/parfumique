import { Link } from "react-router-dom";
import { base64ToUrl } from "../utils";
import { FragranceCardProps } from "../dto-s/Props";
import FragranceActions from "./FragranceActions";
import useDeleteUserFragranceMutation from "../hooks/useDeleteUserFragranceMutation";
import { CircleLoader } from "./loaders/CircleLoader";
import { useQueryClient } from "@tanstack/react-query";

const FragranceCard: React.FC<FragranceCardProps> = ({
  id,
  image,
  name,
  gender,
  onProfile,
}) => {
  const { deleteUserFragranceMutation, deleteFragranceError } =
    useDeleteUserFragranceMutation();
  const onSubmit = async (id: number) => {
    await deleteUserFragranceMutation.mutateAsync({ id });
  };

  return (
    <div className="relative grid gap-4 w-full">
      {deleteUserFragranceMutation.isLoading && <CircleLoader />}
      <div className="rounded-lg border border-gray-200 bg-white p-6 shadow-sm">
        <div className="h-56">
          <a href="#">
            <img
              className="mx-auto h-full rounded-xl"
              src={base64ToUrl(image)}
              alt={`${name} image`}
            />
          </a>
        </div>

        <div className="flex items-center h-12 justify-center">
          <Link
            to={`/fragrances/${id}`}
            className="text-lg text-center font-semibold leading-tight my-text-black"
          >
            {`${name} for ${gender}`}
          </Link>
        </div>

        {!onProfile ? (
          <FragranceActions id={Number(id)} />
        ) : (
          <button
            onClick={() => onSubmit(Number(id))}
            className="flex justify-center items-center mx-auto rounded-md py-2 px-5 my-error"
          >
            Delete fragrance
          </button>
        )}
      </div>
      {deleteFragranceError && (
        <div className="error text-center"> {deleteFragranceError}</div>
      )}
    </div>
  );
};

export default FragranceCard;
