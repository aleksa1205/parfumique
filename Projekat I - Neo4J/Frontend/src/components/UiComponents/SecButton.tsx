type PropsValues = {
    onClick: React.MouseEventHandler<HTMLButtonElement> | undefined;
    children: any;
}

export default function SecButton({onClick, children}: PropsValues) {
    return <button
    onClick={onClick}
    className="font-semibold px-4 py-2 rounded-md cursor-pointer text-brand-600 transition ease-in-out duration-100 border-2 border-brand-600 hover:bg-brand-600 hover:text-white">
        {children}
    </button>
}