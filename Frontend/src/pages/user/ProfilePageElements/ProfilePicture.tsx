import { useContext } from "react";
import { ProfilePageContext, ProfilePageForms } from "../ProfilePage";
import UserImage from "../../../components/UserImage";
import EditButtonPencil from "../../../components/UiComponents/Buttons/EditButtonPencil";

export default function ProfilePicture() {
  const { userChanged, selectForm } = useContext(ProfilePageContext)!;

  return (
    
      <div className="relative w-16 h-16 rounded-full bg-gray-200 md:w-24 md:h-24">
        <UserImage imageString={userChanged?.image} />
        <EditButtonPencil
          onClick={() => selectForm(ProfilePageForms.Picture)}
          className="absolute -bottom-1 -right-1"
        />
    </div>
  )
}