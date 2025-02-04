type PropsValues = {
    onClick?: React.MouseEventHandler<HTMLButtonElement> | undefined;
    children: any;
    disabled?: boolean;
    type?: "submit" | "reset" | "button";
}

export default function MainButton({onClick, children, disabled, type}: PropsValues) {
    return <button
    type={type}
    onClick={onClick}
    disabled={disabled}
    className={`text-center justify-center font-semibold px-4 py-2 rounded-md transition ease-in-out duration-100 border-2 border-brand-600 hover:border-brand-800
                ${disabled ? 'bg-neutral-300 text-neutral-700 cursor-not-allowed border-neutral-300 hover:border-neutral-300' : 'my-active cursor-pointer '}
                flex items-center gap-x-1`}>
    {disabled
    ? (
        <div
            className="size-7 border-4 border-t-brand-700 border-gray-300 rounded-full animate-spin"
        />
    )
    : children}
    </button>
}
