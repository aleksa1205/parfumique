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
    className={`font-semibold px-4 py-2 rounded-md transition ease-in-out duration-100
                ${disabled ? 'bg-neutral-300 text-neutral-700 cursor-not-allowed' : 'my-active cursor-pointer '}
                flex items-center gap-x-1`}>
        {children}
    </button>
}
