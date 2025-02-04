import { useContext } from "react";
import { IoClose } from "react-icons/io5";
import { ModalFormContext } from "../ModalForm";

export default function Header({text}: {
    text: string;
}) {

    const { closeModal } = useContext(ModalFormContext)!;

    return (
        <div className="mb-6 flex justify-between items-center [&>svg]:cursor-pointer">
            <h3 className="text-3xl font-bold">{text}</h3>
            <IoClose onClick={closeModal} size='2rem' />
        </div>
    )
}