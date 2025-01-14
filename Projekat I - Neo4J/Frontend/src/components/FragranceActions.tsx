import { useContext } from "react";
import useIsLoggedIn from "../hooks/useIsLoggedIn";
import { CurrUserContext } from "../context/CurrUserProvider";
import { FragranceActionsProps } from "../dto-s/Props";
import { FaCircleInfo } from "react-icons/fa6";
import useAddFragranceToUserMutation from "../hooks/useAddFragranceToUserMutation";
import { CircleLoader } from "./loaders/CircleLoader";

const FragranceActions = (props: FragranceActionsProps) => {
  const isLoggedIn = useIsLoggedIn();
  const { user } = useContext(CurrUserContext);
  const { addFragranceToUserMutation, addFragranceError } =
    useAddFragranceToUserMutation();

  const isInCollection = user?.collection.some((item) => item.id == props.id);

  const onSubmit = async (props: FragranceActionsProps) => {
    await addFragranceToUserMutation.mutateAsync(props);
  };

  return (
    <div className="min-h-16 pt-5 flex items-center justify-center">
      {addFragranceToUserMutation.isLoading && <CircleLoader />}
      {isLoggedIn &&
        (isInCollection ? (
          <div className="flex items-center justify-center space-x-1 my-text-blue">
            <FaCircleInfo className="my-text-blue text-lg" />
            <span>You already own this</span>
          </div>
        ) : (
          <>
            <button
              onClick={() => onSubmit(props)}
              className="rounded-md py-2 px-5 my-active"
            >
              Add fragrance
            </button>
            {addFragranceError && (
              <div className="error text-center">{addFragranceError}</div>
            )}
          </>
        ))}
    </div>
  );
};

export default FragranceActions;
