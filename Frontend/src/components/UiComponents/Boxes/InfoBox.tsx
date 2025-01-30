import { FaCircleInfo } from "react-icons/fa6";

type PropsVales = {
    children : React.ReactNode;
}

function InfoBox({ children } : PropsVales) {
    return (
        <div 
            className='border-4 border-transparent border-x-blue-600 items-center rounded-lg py-2 px-4 gap-3 flex bg-blue-200 text-blue-600 '
        >
            <FaCircleInfo size='1.25rem' className="shrink-0" />
            <div className="font-bold">
                {children}
            </div>
        </div>
    )
}

export default InfoBox;