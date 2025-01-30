import { useContext } from "react";
import { CurrUserContext } from "../context/CurrUserProvider";
import { base64ToUrl } from "../utils";
import blankProfilePicture from "../assets/images/blank-profile-picture.webp";

const UserImage = () => {
  const { user } = useContext(CurrUserContext);
  const image =
    user?.image && user?.image != ""
      ? base64ToUrl(user?.image)
      : blankProfilePicture;
  return (
    <img src={image} alt="user profile picture" className="rounded-full" />
  );
};

export default UserImage;
