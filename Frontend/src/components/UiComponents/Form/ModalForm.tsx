import { IoClose } from "react-icons/io5";
import { MdErrorOutline } from "react-icons/md";
import { FieldErrors, FieldValues, UseFormRegister, Path, RegisterOptions } from "react-hook-form";
import MainButton from "../Buttons/MainButton";
import SecButton from "../Buttons/SecButton";

export default function ModalForm({ onSubmit, children }: {
    onSubmit: () => void,
    children: any
}) {
    return (
        <form 
            className="py-6 px-8 w-[46rem]"
            noValidate
            onSubmit={onSubmit}
        >
            {children}
        </form>
    )
}

ModalForm.Header = function ({text, closeModal}: {
    text: string;
    closeModal: () => void;
}) {
    return (
        <div className="mb-6 flex justify-between items-center [&>svg]:cursor-pointer">
            <h3 className="text-3xl font-bold">{text}</h3>
            <IoClose onClick={closeModal} size='2rem' />
        </div>
    )
}

ModalForm.Description = function ({text}:{
    text: string;
}) {
    return (
        <p>{text}</p>
    )
}

ModalForm.ButtonGroup = function ({closeModal, isSubmitting}: {
    closeModal: () => void;
    isSubmitting: boolean
}) {
    return (
        <div className="mt-[1.5rem] flex gap-[1rem] flex-row-reverse">
            <MainButton type="submit" disabled={isSubmitting}>
                Save
            </MainButton>
            <SecButton type="button" onClick={closeModal}>
                Cancel
            </SecButton>
        </div>
    )
}

ModalForm.TextInput = function<T extends FieldValues> ({ name, labelText, placeholder, errors, register, options }: {
    name: Path<T>;
    labelText: string;
    placeholder: string;
    register: UseFormRegister<T>;
    errors: FieldErrors<T>,
    options?: RegisterOptions<T, Path<T>>
}) {
    return (
        <div className="mt-[1.5rem]">
            <label
                htmlFor={String(name)}
                className="block mb-1 font-bold"
            >
                {labelText}
            </label>
            <input
                type="text"
                id={String(name)}
                placeholder={placeholder}
                className="mt-3 w-full p-2 border rounded-lg transition-all duration-150 hover:ring-2 hover:ring-brand-700 focus:ring-2 focus:ring-brand-500 focus:outline-none"
                {...register(name, options)}
            />
            <p className="text-red-500 flex gap-1 items-center font-bold mb-4">
                {errors[name]?.message && <MdErrorOutline />}
                {errors[name]?.message as string}
            </p>
        </div>
    )
}