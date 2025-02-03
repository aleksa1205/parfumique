type PropsValues = {
    onClick?: React.MouseEventHandler<HTMLButtonElement> | undefined;
    children: any;
    disabled?: boolean;
    type?: "submit" | "reset" | "button";
}

export default function SecButton({onClick, children, disabled, type}: PropsValues) {
    
    const disabledClasses = 'cursor-not-allowed text-neutral-700 border-neutral-700';
    const enabledClasses = 'text-brand-600 border-brand-600 hover:bg-brand-600 hover:text-white cursor-pointer'
    
    return <button
    type={type}
    onClick={onClick}
    disabled={disabled}
    className={`font-semibold px-4 py-2 rounded-md transition ease-in-out duration-100 border-2 
                ${disabled ? disabledClasses : enabledClasses}`}>
        {children}
    </button>
}