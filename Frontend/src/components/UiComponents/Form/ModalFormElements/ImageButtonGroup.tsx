import { useContext } from "react";
import { ModalFormContext } from "../ModalForm";
import MainButton from "../../Buttons/MainButton";
import SecButton from "../../Buttons/SecButton";
import { MdFileUpload } from "react-icons/md";

export default function ImageButtonGroup({isSubmitting, valueFromForm}: {
    isSubmitting: boolean;
    valueFromForm: FileList;
}) {
    const { closeModal } = useContext(ModalFormContext)!;

    return (
        <div className="grid grid-cols-2 mt-[1.5rem] md:flex gap-[1rem] flex-row-reverse">
            {typeof valueFromForm !== "undefined" && valueFromForm.length !== 0
            ? (
                <>
                    <label
                        htmlFor="image"
                        className="justify-center flex text-brand-600 border-brand-600 hover:bg-brand-600 hover:text-white cursor-pointer
                                   font-semibold px-4 py-2 rounded-md transition ease-in-out duration-100 border-2"
                    >
                        Change Image
                    </label>
                    <MainButton disabled={isSubmitting}>
                        Save
                    </MainButton>
                </>
            ) : (
                <>
                    <SecButton type="button" onClick={closeModal}>
                        Cancel
                    </SecButton>
                    <label 
                        htmlFor="image"
                        className="justify-center font-semibold px-4 py-2 rounded-md transition ease-in-out duration-100 my-active cursor-pointer flex items-center gap-x-2 [&>svg]:shrink-0"
                    >
                        <MdFileUpload size="1rem" />
                        Upload Image
                    </label>
                </>
            )}
 
        </div>
    )
}