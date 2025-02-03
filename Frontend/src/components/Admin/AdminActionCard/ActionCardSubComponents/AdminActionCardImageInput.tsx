import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { base64ToUrl, crop, formatBase64 } from "../../../../utils";
import { Loader } from "../../../loaders/Loader";
import { MdErrorOutline, MdFileUpload } from "react-icons/md";
import InfoBox from "../../../UiComponents/Boxes/InfoBox";
import MainButton from "../../../UiComponents/Buttons/MainButton";

type FromValues = {
    value: FileList;
};

export function AdminActionCardImageInput({setImageValue, imageValue}: {
    setImageValue: React.Dispatch<React.SetStateAction<string>>
    imageValue: string
}) {
    const form = useForm<FromValues>();
  const { register, handleSubmit, formState, watch } = form;
  const { errors } = formState;

  const [image, setImage] = useState<string | null>(null);
  const [isCropping, setIsCropping] = useState<boolean>(false);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const inputValue = watch("value");

  useEffect(() => {
    setImageValue('');
    setImage(null)
  }, [])

  useEffect(() => {
    function loadPicture() {
        if (imageValue !== "") {
          setIsLoading(true);
          setImage(base64ToUrl(imageValue));
          setIsLoading(false);
        }
        else
          setImage(null);
    }
    async function startCropping() {
      if (typeof inputValue !== "undefined" && inputValue.length !== 0) {
        setIsCropping(true);
        try {
          const imageUrl = URL.createObjectURL(inputValue[0]);
          const res = await crop(imageUrl, 1);
          const url = base64ToUrl(formatBase64(res.toDataURL()));
          setImage(url);
        } catch (error) {
          console.error("Error cropping image:", error);
        } finally {
          setIsCropping(false);
        }
      }
    }
    
    loadPicture();
    startCropping();
  }, [inputValue]);

  async function onSubmit(formValues: FromValues) {
    const { value } = formValues;

    try {
      const croppedCanvas = await crop(URL.createObjectURL(value[0]), 1);

      const croppedBase64Image = croppedCanvas.toDataURL("image/jpeg");

      setImageValue((formatBase64(croppedBase64Image)));
    } catch (error) {
      console.error("Failed to crop or convert the image:", error);
    }
  }

  return (
    <form 
        className='' 
        noValidate onSubmit={handleSubmit(onSubmit)}
    >

    <h3 className='font-lg font-bold text-gray-700 mb-4'>Upload an image</h3>

      <div>
          {isCropping ? (
            <div className='my-5'>
              <Loader />
            </div>
          ) : image !== null ? (
            <div className='my-5'>
              <img className='max-w-sm' src={image} alt="image" />
            </div>
          ) : (
            <></>
          )}
          <input
            className='w-0 h-0 opacity-0 -z-1 absolute'
            type="file"
            accept="image/png, image/jpeg"
            id="slika"
            {...register("value", {
              required: "You need to upload the image first",
              validate: function (fieldValue) {
                const info = fieldValue[0].size <= 204800;
                return info || "Image is larger then 200KB";
              },
            })}
          />
          <p className='text-red-500 font-bold mb-4 flex items-center gap-x-1'>
            {errors.value?.message && <MdErrorOutline />}
            {errors.value?.message}
          </p>

        {isLoading && <p>Učitavanje profilne slike...</p>}

        <InfoBox>
            <p>The image is automatically cropped to a 1:1 ratio.</p>
            <p>You need to upload the image and click "Save" for it to be sent to the server.</p>
        </InfoBox>
      </div>
      <div className='flex mt-5 gap-x-5'>
        <label
            className='font-semibold px-4 py-2 rounded-md transition ease-in-out duration-100 my-active cursor-pointer flex items-center gap-x-2'
            htmlFor="slika"
        >
            <MdFileUpload size="1rem" />
            Upload Image
        </label>

        <MainButton onClick={() => {}}>
            Save
        </MainButton>
      </div>
    </form>
  );
}