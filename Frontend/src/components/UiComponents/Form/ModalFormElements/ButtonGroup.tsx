import { useContext } from "react";
import { ModalFormContext } from "../ModalForm";
import MainButton from "../../Buttons/MainButton";
import SecButton from "../../Buttons/SecButton";

export default function ButtonGroup ({isSubmitting}: {
    isSubmitting: boolean
}) {
    const { closeModal } = useContext(ModalFormContext)!;

    return (
        <div className="grid grid-cols-2 mt-[1.5rem] md:flex gap-[1rem] flex-row-reverse">
            <MainButton type="submit" disabled={isSubmitting}>
                Save
            </MainButton>
            <SecButton type="button" onClick={closeModal}>
                Cancel
            </SecButton>
        </div>
    )
}