type PropsValues = {
    onClick: React.MouseEventHandler<HTMLButtonElement> | undefined;
    children: any;
}

export default function MainButton({onClick, children}: PropsValues) {
    return <button
    onClick={onClick}
    className="block py-2 px-3 rounded-2xl my-active transition duration-100">
        {children}
    </button>
}