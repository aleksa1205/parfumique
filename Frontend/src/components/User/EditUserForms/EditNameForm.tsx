import { useForm } from "react-hook-form";
import ModalForm from "../../UiComponents/Form/ModalForm";
import { useContext } from "react";
import { ProfilePageContext } from "../../../pages/user/ProfilePage";

type FormValues = {
    name: string;
}

const validation = {
    required: "Name is a required field.",
    minLength: {
        value: 3,
        message: "Name must be at least 3 characters long."
    },
    maxLength: {
        value: 15,
        message: "Name cannot exceed 15 characters."
    },
    pattern: {
        value: /^[a-zA-ZčćžđšČĆŽĐŠ]+$/,
        message: "Name can only contain letters."
    }
}

export default function EditNameForm() {
    const { setUserChanged, closeModal, userChanged } = useContext(ProfilePageContext)!;

    const { register, handleSubmit, formState } = useForm<FormValues>({ defaultValues: {
        "name": userChanged?.name
    } });

    const { isSubmitting, errors, isValid } = formState;

    async function onSubmit({ name }: FormValues) {
        setUserChanged(prev => ({...prev!, name}))
        if(isValid) closeModal();   
    }

    return (
        <ModalForm onSubmit={handleSubmit(onSubmit)} closeModal={closeModal}>
            <ModalForm.Header text="Change your Name" />
            <ModalForm.Description text="Do not enter a middle name or nickname. Please enter your full first name." />
            <ModalForm.TextInput<FormValues>
                errors={errors}
                labelText="Enter your name"
                name="name"
                placeholder="eg. Nate"
                register={register}
                options={validation}                
            />
            <ModalForm.ButtonGroup isSubmitting={isSubmitting} />
        </ModalForm>
    )
}