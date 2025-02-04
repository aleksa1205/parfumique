import { createContext, useContext, useEffect, useState } from "react";
import { CurrUserContext } from "../../context/CurrUserProvider";
import { GetUserResponse } from "../../dto-s/UserDto";
import useAnimatedModal from "../../hooks/Animated Components/useAnimatedModal";
import EditNameForm from "../../components/User/EditUserForms/EditNameForm";
import InfoBox from "../../components/UiComponents/Boxes/InfoBox";
import EditSurnameForm from "../../components/User/EditUserForms/EditSurnameForm";
import EditGenderForm from "../../components/User/EditUserForms/EditGenderForm";
import EditPictureForm from "../../components/User/EditUserForms/EditPicture";
import { Loader } from "../../components/loaders/Loader";
import usePopUpMessage, { PopUpMessage } from "../../hooks/Animated Components/usePopUpMessage";
import MainContainer from "./ProfilePageElements/MainContainer";
import ProfileCardContainer from "./ProfilePageElements/ProfileCardContainer";
import GridContainer from "./ProfilePageElements/GridContainer";
import Header from "./ProfilePageElements/Header";
import ProfilePicture from "./ProfilePageElements/ProfilePicture";
import Name from "./ProfilePageElements/Name";
import Surname from "./ProfilePageElements/Surname";
import Gender from "./ProfilePageElements/Gender";
import SaveResetButtons from "./ProfilePageElements/SaveResetButtons";

type ProfilePageContextValue = {
  userChanged?: GetUserResponse;
  user?: GetUserResponse;
  setUserChanged: React.Dispatch<React.SetStateAction<GetUserResponse | undefined>>;
  closeModal: () => void;
  selectForm: (form: ProfilePageForms) => void;
  isChanged: boolean;
  isUserDataLoading: boolean;
  refetch: () => void;
  setPopUpMessage: React.Dispatch<React.SetStateAction<PopUpMessage | null>>;
}

export const ProfilePageContext = createContext<ProfilePageContextValue | null>(null)

export enum ProfilePageForms {
  None,
  Name,
  Surname,
  Gender,
  Picture
}

const ProfilePage = () => {
  const { user, isLoading, refetch } = useContext(CurrUserContext);
  const [isChanged, setIsChanged] = useState(false);
  const [userChanged, setUserChanged] = useState<GetUserResponse | undefined>(user);

  const { AnimatedModal, closeModal, openModal } = useAnimatedModal();

  const [selectedForm, setSelectedForm] = useState<ProfilePageForms>(ProfilePageForms.None);
  
  const { PopUpComponent, setPopUpMessage } = usePopUpMessage();

  useEffect(() => {
    setUserChanged(user);
  }, [user]);

  useEffect(() => {
    setIsChanged(JSON.stringify(user) !== JSON.stringify(userChanged))
  }, [userChanged])

  let selectedFormComponent;
  switch (selectedForm) {
    case ProfilePageForms.Name:
      selectedFormComponent = <EditNameForm />
      break;
    case ProfilePageForms.Surname:
      selectedFormComponent = <EditSurnameForm />
      break;
    case ProfilePageForms.Gender:
      selectedFormComponent = <EditGenderForm />
      break;
    case ProfilePageForms.Picture:
      selectedFormComponent = <EditPictureForm />
      break;
    default:
      selectedFormComponent = null;
      break;
  }

  function selectForm(form: ProfilePageForms) {
    setSelectedForm(form);
    openModal();
  }

  if (isLoading) return <Loader />

  return (
    <ProfilePageContext.Provider value={{userChanged, user: user!, setUserChanged, closeModal, selectForm, isChanged, isUserDataLoading: isLoading, refetch, setPopUpMessage}}>
      <PopUpComponent />
      <AnimatedModal>
        {selectedFormComponent}
      </AnimatedModal>

      <ProfilePage.MainContainer>

        {(isChanged) && (
          <div className="max-w-xs mx-auto mb-4">
            <InfoBox>
              You have unsaved changes.
            </InfoBox>
          </div>
        )}

        <ProfilePage.ProfileCardContainer>

          <ProfilePage.Header />
          
          <ProfilePage.GridContainer>
            <ProfilePage.ProfilePicture />
            <div className="flex flex-col space-y-6">
              <ProfilePage.Name />
              <ProfilePage.Surname />
              <ProfilePage.Gender />
            </div>
          </ProfilePage.GridContainer>

          <ProfilePage.SaveResetButtons />

        </ProfilePage.ProfileCardContainer>
      </ProfilePage.MainContainer>

    </ProfilePageContext.Provider>
  );
};

export default ProfilePage;

ProfilePage.MainContainer = MainContainer;
ProfilePage.ProfileCardContainer = ProfileCardContainer;
ProfilePage.GridContainer = GridContainer;
ProfilePage.Header = Header;
ProfilePage.ProfilePicture = ProfilePicture; 
ProfilePage.Name = Name;
ProfilePage.Surname = Surname;
ProfilePage.Gender = Gender
ProfilePage.SaveResetButtons = SaveResetButtons;