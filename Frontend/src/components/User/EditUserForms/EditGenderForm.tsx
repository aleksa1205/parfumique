import { RegisterOptions, useForm } from "react-hook-form";
import ModalForm from "../../UiComponents/Form/ModalForm";
import { useContext } from "react";
import { ProfilePageContext } from "../../../pages/user/ProfilePage";

type FormValues = {
    gender: string;
}

const validation: RegisterOptions<FormValues> = {
    required: "Name is a required field.",
}

export default function EditGenderForm() {
    const { setUserChanged, closeModal, userChanged } = useContext(ProfilePageContext)!;

    const { register, handleSubmit, formState } = useForm<FormValues>({ defaultValues: {
        "gender": userChanged?.gender
    } });

    const { isSubmitting, errors, isValid } = formState;

    async function onSubmit({ gender }: FormValues) {
        setUserChanged(prev => ({...prev!, gender}))
        if(isValid) closeModal();   
    }

    return (
        <ModalForm onSubmit={handleSubmit(onSubmit)} closeModal={closeModal}>
            <ModalForm.Header text="Change your Gender" />
            <ModalForm.Description text="Please select your gender." />
            <ModalForm.DropdownInput<FormValues>
                errors={errors}
                labelText="Enter your gender"
                name="gender"
                possibleValues={["M", "F"]}
                register={register}
                options={validation}                
            />
            <ModalForm.ButtonGroup isSubmitting={isSubmitting} />
        </ModalForm>
    )
}