import { useContext } from "react";
import EditButtonPencil from "../../../components/UiComponents/Buttons/EditButtonPencil";
import PencilButtonContainer from "../../../components/UiComponents/Buttons/PencilButtonContainer";
import { ProfilePageContext, ProfilePageForms } from "../ProfilePage";

export default function Name() {
  const { userChanged, selectForm } = useContext(ProfilePageContext)!;

  return (
    <PencilButtonContainer className="space-x-8">
    <div className="flex items-center space-x-2">
      <span className="text-md font-light text-neutral-600">Name:</span>
      <span className="text-xl text-neutral-900">{userChanged?.name}</span>
    </div>
    <EditButtonPencil onClick={() => selectForm(ProfilePageForms.Name)} />
  </PencilButtonContainer>
  )
}