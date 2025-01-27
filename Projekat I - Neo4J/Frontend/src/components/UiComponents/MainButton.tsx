type PropsValues = {
  onClick: React.MouseEventHandler<HTMLButtonElement> | undefined;
  children: any;
};

export default function MainButton({ onClick, children }: PropsValues) {
  return (
    <button
      onClick={onClick}
      className="font-semibold px-4 py-2 rounded-md cursor-pointer my-active transition ease-in-out duration-100"
    >
      {children}
    </button>
  );
}
