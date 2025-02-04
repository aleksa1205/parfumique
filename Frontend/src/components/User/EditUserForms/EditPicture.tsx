import { useForm } from "react-hook-form";
import ModalForm from "../../UiComponents/Form/ModalForm";
import { useContext } from "react";
import { ProfilePageContext } from "../../../pages/user/ProfilePage";
import { crop, formatBase64 } from "../../../utils";
import InfoBox from "../../UiComponents/Boxes/InfoBox";

type FormValues = {
    image: FileList;
}

export default function EditPictureForm() {
    const { setUserChanged, closeModal, userChanged } = useContext(ProfilePageContext)!;

    const { register, handleSubmit, formState, watch } = useForm<FormValues>();

    const { isSubmitting, errors, isValid } = formState;

    const valueFromForm = watch('image');

    async function onSubmit({ image: value }: FormValues) {
        try {
            const croppedCanvas = await crop(URL.createObjectURL(value[0]), 1);
      
            const croppedBase64Image = croppedCanvas.toDataURL("image/jpeg");
      
            setUserChanged((prev) => ({
              ...prev!,
              image: formatBase64(croppedBase64Image),
            }));

            if(isValid) closeModal();
          } catch (error) {
            console.error("Failed to crop or convert the image:", error);
          } 
    }

    return (
        <ModalForm onSubmit={handleSubmit(onSubmit)} closeModal={closeModal}>
            <ModalForm.Header text="Change your Profile Picture" />
            <ModalForm.ScrollContainer>
                <ModalForm.ImageContentContainer>
                    <ModalForm.ImageInputHolder<FormValues>
                        image={userChanged?.image}
                        valueFromForm={valueFromForm}
                        errors={errors}
                        name="image"
                        register={register}
                    />
                </ModalForm.ImageContentContainer>

                <InfoBox>
                    <p>The image is automatically cropped to a 1:1 ratio.</p>
                </InfoBox>
            </ModalForm.ScrollContainer>
            <ModalForm.ImageButtonGroup valueFromForm={valueFromForm} isSubmitting={isSubmitting} />
        </ModalForm>
    )
}