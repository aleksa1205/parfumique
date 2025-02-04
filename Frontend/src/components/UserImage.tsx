import { useContext } from "react";
import { CurrUserContext } from "../context/CurrUserProvider";
import { base64ToUrl } from "../utils";
import blankProfilePicture from "../assets/images/blank-profile-picture.webp";

const UserImage = ({imageString}: {
  imageString?: string;
}) => {
  const { user } = useContext(CurrUserContext);

  let image;
  if (!imageString) {
    image =
      user?.image && user?.image != ""
        ? base64ToUrl(user?.image)
        : blankProfilePicture;
  }
  else {
    image =
      imageString != ""
        ? base64ToUrl(imageString)
        : blankProfilePicture;
  }

  return (
    <img src={image} alt="user profile picture" className="rounded-full" />
  );
};

export default UserImage;
