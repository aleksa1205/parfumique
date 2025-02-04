import { LuPencil } from "react-icons/lu";

export default function EditButtonPencil({ onClick, className }: {
    onClick: () => void;
    className?: string;
}) {
    return (
        <button
            onClick={onClick}
            className={`bg-white border-2 border-brand-500 rounded-full text-brand-500 cursor-pointer flex items-center justify-center p-0 m-0 w-8 h-8 transition duration-[0.1s] ease-in-out shrink-0
                       hover:bg-brand-200 hover:text-brand-600 hover:border-brand-600
                       active:bg-brand-300 active:scale-90 ${className}`}
        >
            <LuPencil size='1rem' />
        </button>
    )
}