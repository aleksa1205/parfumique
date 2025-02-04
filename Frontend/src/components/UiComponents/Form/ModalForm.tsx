import { createContext } from "react";
import Header from "./ModalFormElements/Header";
import Description from "./ModalFormElements/Description";
import ImageInputHolder from "./ModalFormElements/ImageInputHolder";
import ImageButtonGroup from "./ModalFormElements/ImageButtonGroup";
import ScrollContainer from "./ModalFormElements/ScrollContainer";
import ImageContentContainer from "./ModalFormElements/ImageContentContainer";
import ButtonGroup from "./ModalFormElements/ButtonGroup";
import TextInput from "./ModalFormElements/TextInput";
import DropdownInput from "./ModalFormElements/DropDownInput";

type ContextValues = {
    closeModal: () => void;
}

export const ModalFormContext = createContext<ContextValues | null>(null);

export default function ModalForm({ onSubmit, children, closeModal }: {
    onSubmit: () => void,
    children: any;
    closeModal: () => void;
}) {
    return (
        <ModalFormContext.Provider value={{closeModal}}>
            <form 
                className="py-6 px-8 w-[48rem]"
                noValidate
                onSubmit={onSubmit}
            >
                {children}
            </form>
        </ModalFormContext.Provider>
    )
}

ModalForm.Header = Header
ModalForm.Description = Description
ModalForm.ImageInputHolder = ImageInputHolder
ModalForm.ImageButtonGroup = ImageButtonGroup
ModalForm.ScrollContainer = ScrollContainer
ModalForm.ImageContentContainer = ImageContentContainer
ModalForm.ButtonGroup = ButtonGroup
ModalForm.TextInput = TextInput
ModalForm.DropdownInput = DropdownInput