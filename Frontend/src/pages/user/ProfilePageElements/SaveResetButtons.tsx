import { useContext, useState } from "react";
import useUserController from "../../../api/controllers/useUserController";
import { ProfilePageContext } from "../ProfilePage";
import MainButton from "../../../components/UiComponents/Buttons/MainButton";
import SecButton from "../../../components/UiComponents/Buttons/SecButton";

export default function() {
    const { isChanged, user, setUserChanged, userChanged, refetch, setPopUpMessage } = useContext(ProfilePageContext)!;
    const { updateSelf } = useUserController();
    const [isFetching, setIsFetching] = useState(false);
    
    if (!isChanged) return <></>
  
    async function onSubmit() {
      try {
        if (!userChanged) throw new Error("User data is not present.");
        
        setIsFetching(true);
        await updateSelf({
          name: userChanged.name,
          gender: userChanged.gender,
          image: userChanged.image,
          surname: userChanged.surname,
        })
        refetch();
        setIsFetching(false);
        setPopUpMessage({
          message: "Your profile is updated successfully",
          type: 'success'
        })
      } catch (error) {
        const errorMess = 
          error instanceof Error
          ? error.message
          : ""
        console.error("Error while calling update self" + errorMess);
        setPopUpMessage({
          message: "Error while trying to update your profile.",
          type: 'error',
          dontClose: true
        })
      }
      finally {
        setIsFetching(false);
      }
    }
  
    return (
      <div className="flex items-center flex-row-reverse gap-4 mt-8">
        <MainButton
          onClick={() => onSubmit()}
          disabled={isFetching}
        >
          Save
        </MainButton>
  
        <SecButton
          onClick={() => setUserChanged(user)}
        >
          Reset
        </SecButton>
      </div>
    )
  }