import { animated } from "@react-spring/web";
import { IoClose } from "react-icons/io5";
import { useEffect } from "react";
import { IoIosWarning } from "react-icons/io";

type PropsVales = {
    children : React.ReactNode;
    style: any;
    closeMessage: () => void;
    dontClose?: boolean;
}

export default function WarningBoxAnimated({ children, style, closeMessage, dontClose = false } : PropsVales) {
    
    useEffect(() => {
        if (dontClose) return;

        const id = setTimeout(closeMessage, 5000);

        return () => {
            clearTimeout(id);
        }
    }, [])

    return (
        <animated.div style={style}
            className="rounded-lg py-3 px-4 flex items-center gap-4 fixed left-1/2 z-999 [&>svg]:shrink-0 [&>svg]grow-0 pr-12
                       bg-yellow-500 text-white"
        >
            <IoIosWarning size='1.25rem'/>
            <div
                className="[&>p]:font-bold [&>p]:margin-0"
            >
                {children}
            </div>
            <IoClose onClick={closeMessage} className="absolute right-3 cursor-pointer" size='1.75rem' />
        </animated.div>
    )
}