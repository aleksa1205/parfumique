type PropsValue = {
  onClick: React.MouseEventHandler<HTMLButtonElement>;
};

const DeleteButton = ({ onClick }: PropsValue) => {
  return (
    <button
      onClick={onClick}
      className="flex justify-center items-center mx-auto rounded-md py-2 px-5 my-error"
    >
      Delete
    </button>
  );
};

export default DeleteButton;
