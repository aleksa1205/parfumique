import { ButtonProps } from "../../dto-s/Props";

const DeleteButton: React.FC<ButtonProps> = ({ func, id }) => {
  return (
    <button
      onClick={() => func(id)}
      className="flex justify-center items-center mx-auto rounded-md py-2 px-5 my-error"
    >
      Delete
    </button>
  );
};

export default DeleteButton;
