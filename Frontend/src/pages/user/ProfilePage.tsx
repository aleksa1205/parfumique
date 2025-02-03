import { useContext } from "react";
import { CurrUserContext } from "../../context/CurrUserProvider";
import UserImage from "../../components/UserImage";
import useAnimatedModal from "../../hooks/Animated Components/useAnimatedModal";
import ModalForm from "../../components/UiComponents/Form/ModalForm";
import { useForm } from "react-hook-form";

const ProfilePage = () => {
  const { user } = useContext(CurrUserContext);

  const { register, handleSubmit, formState } = useForm<{name: string}>();
  const { errors, isSubmitting } = formState;

  const { AnimatedModal, openModal, closeModal } = useAnimatedModal()

  function onSubmit(formValues: {name: string}) {
    const { name } = formValues;
    console.log(name)
  }

  return (
    <>

    <AnimatedModal>
      <ModalForm onSubmit={handleSubmit(onSubmit)}>
        <ModalForm.Header closeModal={closeModal} text="Header" />
        <ModalForm.Description text="description" />
        <ModalForm.TextInput<{name: string}> errors={errors} labelText="name" name="name" placeholder="name" register={register} />
        <ModalForm.ButtonGroup closeModal={closeModal} isSubmitting={isSubmitting} />
      </ModalForm>
    </AnimatedModal>

    <div className="flex justify-center items-start bg-gray-100 py-8 mt-24">
      <button onClick={() => openModal()}>open</button>
      <div className="flex items-center space-x-4 p-6 bg-white border rounded-lg shadow-md">
        <div className="w-16 h-16 rounded-full overflow-hidden bg-gray-200 xl:w-24 xl:h-24">
          <UserImage />
        </div>

        <div className="flex flex-col space-y-6">
          <h1 className="text-3xl font-semibold text-gray-900 mb-4">
            {user?.username}
          </h1>

          <div className="flex items-center space-x-2 mb-2">
            <span className="text-sm font-medium text-gray-600">Name:</span>
            <span className="text-lg text-gray-900">{user?.name}</span>
          </div>

          <div className="flex items-center space-x-2 mb-2">
            <span className="text-sm font-medium text-gray-600">Surname:</span>
            <span className="text-lg text-gray-900">{user?.surname}</span>
          </div>

          <div className="flex items-center space-x-2">
            <span className="text-sm font-medium text-gray-600">Gender:</span>
            <span className="text-lg text-gray-900">
              {user?.gender ? user.gender : "Not specified"}
            </span>
          </div>
        </div>
      </div>
    </div>
    </>
  );
};

export default ProfilePage;
