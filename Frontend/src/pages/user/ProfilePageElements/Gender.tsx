import { useContext } from "react";
import { ProfilePageContext, ProfilePageForms } from "../ProfilePage";
import PencilButtonContainer from "../../../components/UiComponents/Buttons/PencilButtonContainer";
import EditButtonPencil from "../../../components/UiComponents/Buttons/EditButtonPencil";

export default function Gender() {
  const { userChanged, selectForm } = useContext(ProfilePageContext)!;
  
  return (
    <PencilButtonContainer>
      <div className="flex items-center space-x-2">
        <span className="text-md font-light text-neutral-600 ">Gender:</span>
        <span className="text-xl text-neutral-900">
          {userChanged?.gender ? userChanged.gender : "Not specified"}
        </span>
      </div>
      <EditButtonPencil onClick={() => selectForm(ProfilePageForms.Gender)} />
    </PencilButtonContainer>
  )
}