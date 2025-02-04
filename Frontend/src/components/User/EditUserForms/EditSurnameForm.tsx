import { useForm } from "react-hook-form";
import ModalForm from "../../UiComponents/Form/ModalForm";
import { useContext } from "react";
import { ProfilePageContext } from "../../../pages/user/ProfilePage";

type FormValues = {
    surname: string;
}

const validation = {
    required: "Surname is a required field.",
    minLength: {
        value: 3,
        message: "Surname must be at least 3 characters long."
    },
    maxLength: {
        value: 15,
        message: "Surname cannot exceed 15 characters."
    },
    pattern: {
        value: /^[a-zA-ZčćžđšČĆŽĐŠ]+$/,
        message: "Surname can only contain letters."
    }
}

export default function EditSurnameForm() {
    const { setUserChanged, closeModal, userChanged } = useContext(ProfilePageContext)!;

    const { register, handleSubmit, formState } = useForm<FormValues>({ defaultValues: {
        "surname": userChanged?.surname
    } });

    const { isSubmitting, errors, isValid } = formState;

    async function onSubmit({ surname }: FormValues) {
        setUserChanged(prev => ({...prev!, surname}))
        if(isValid) closeModal();   
    }

    return (
        <ModalForm onSubmit={handleSubmit(onSubmit)} closeModal={closeModal}>
            <ModalForm.Header text="Change your Surname" />
            <ModalForm.Description text="Do not enter a middle name or nickname. Please enter your full last name." />
            <ModalForm.TextInput<FormValues>
                errors={errors}
                labelText="Enter your surnname"
                name="surname"
                placeholder="eg. Higgers"
                register={register}
                options={validation}                
            />
            <ModalForm.ButtonGroup isSubmitting={isSubmitting} />
        </ModalForm>
    )
}