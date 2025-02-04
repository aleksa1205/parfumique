import { FaCircleInfo } from "react-icons/fa6";

type PropsVales = {
    children : React.ReactNode;
    className?: string;
}

function InfoBox({ children, className } : PropsVales) {
    return (
        <div 
            className={`w-fill border-4 border-transparent border-x-blue-600 items-center rounded-lg py-2 px-4 gap-3 flex bg-blue-200 text-blue-600 ${className}`}
        >
            <FaCircleInfo size='1.25rem' className="shrink-0" />
            <span className="font-bold">
                {children}
            </span>
        </div>
    )
}

export default InfoBox;