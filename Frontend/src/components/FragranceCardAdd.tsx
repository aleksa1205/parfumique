import { useContext } from "react";
import { CurrUserContext } from "../context/CurrUserProvider";
import useAddFragranceToUserMutation from "../hooks/useAddFragranceToUserMutation";
import { CircleLoader } from "./loaders/CircleLoader";
import FragranceCard from "./FragranceCard";
import useIsLoggedIn from "../hooks/useIsLoggedIn";
import { FaCircleInfo } from "react-icons/fa6";
import MainButton from "./UiComponents/Buttons/MainButton";
import { BaseFragrance } from "../dto-s/FragranceDto";

const FragranceCardAdd = (props: BaseFragrance) => {
  const isLoggedIn = useIsLoggedIn();
  const { user } = useContext(CurrUserContext);
  const { addFragranceToUserMutation, addFragranceError } =
    useAddFragranceToUserMutation();
  const isInCollection = user?.collection.some(
    (item) => item.id == Number(props.id)
  );
  const onAdd = async () => {
    await addFragranceToUserMutation.mutateAsync(props.id);
  };

  if (addFragranceToUserMutation.isLoading) {
    return <CircleLoader />;
  }
  return (
    <div className="relative grid gap-4 w-full">
      <FragranceCard {...props} />
      <div className="min-h-16 pt-3 flex items-center justify-center">
        {isLoggedIn &&
          (isInCollection ? (
            <div className="flex items-center justify-center space-x-1 my-text-blue">
              <FaCircleInfo className="my-text-blue text-lg" />
              <span>You already own this</span>
            </div>
          ) : (
            <>
              <MainButton onClick={() => onAdd()}>Add</MainButton>
              {addFragranceError && (
                <div className="error text-center">{addFragranceError}</div>
              )}
            </>
          ))}
      </div>
    </div>
  );
};

export default FragranceCardAdd;
