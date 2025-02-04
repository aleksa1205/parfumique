import { useContext } from "react";
import { ProfilePageContext } from "../ProfilePage";

export default function Header() {
  const { userChanged } = useContext(ProfilePageContext)!;
  
  return (
    <h1 className="text-3xl font-semibold text-gray-900 mb-10 text-center">
      {userChanged?.username}
    </h1>
  )
}