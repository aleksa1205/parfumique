import { MdErrorOutline } from "react-icons/md";
import { SpinnerBlade } from "../../../loaders/SpnnerBlade";
import { base64ToUrl, crop, formatBase64 } from "../../../../utils";
import { useEffect, useState } from "react";
import { FieldErrors, FieldValues, Path, UseFormRegister } from "react-hook-form";

export default function ImageInputHolder<T extends FieldValues> ({ image, valueFromForm, name, errors, register, description}: {
    image?: string;
    valueFromForm: FileList;
    name: Path<T>;
    register: UseFormRegister<T>;
    errors: FieldErrors<T>,
    description?: string;
}) {
    const [imageConverted, setImageConverted] = useState<string | null>(null);
    const [isCropping, setIsCropping] = useState<boolean>(false);
    const [isLoading, setIsLoading] = useState<boolean>(false);

    useEffect(() => {
        function loadPicture() {
          if (image && image !== "") {
            setIsLoading(true);
            setImageConverted(base64ToUrl(image));
            setIsLoading(false);
          }
        }
        async function startCropping() {
          if (typeof valueFromForm !== "undefined" && valueFromForm.length !== 0) {
            setIsCropping(true);
            try {
              const imageUrl = URL.createObjectURL(valueFromForm[0]);
              const res = await crop(imageUrl, 1);
              const url = base64ToUrl(formatBase64(res.toDataURL()));
              setImageConverted(url);
            } catch (error) {
              console.error("Error cropping image:", error);
            } finally {
              setIsCropping(false);
            }
          }
        }
    
        loadPicture();
        startCropping();
      }, [valueFromForm]);

    return (
        <>
            {isCropping
            ? (
                <div className="mb-4">
                    <SpinnerBlade />
                </div>
            ) : (imageConverted !== '' && imageConverted !== null)
            ? (
                <img className="mb-4 w-[60%] md:w-full rounded-full" src={imageConverted} alt="image" />
            ) : (
                <div className="w-[50%] pt-[50%] mb-8 md:mb-0 md:w-full md:pt-[100%] relative">
                    <div className="rounded-full absolute inset-0 border-[3px] border-dashed border-brand-500 bg-neutral-100 text-neutral-400
                                    flex justify-center items-center">
                        <span className="text-md sm:text-2xl md:text-3xl">Upload an image</span>
                    </div>
                </div>
            )}
            <p className="text-lg font-medium">
                {description
                ? description
                : "Upload favourite picture of yourself!"}
            </p>
            <input
                className="size-[0.1px] opacity-0 overflow-hidden absolute -z-1"
                type="file"
                accept="image/png, image/jpeg"
                id="image"
                {...register(name, {
                    required: "This is required field.",
                    validate: function (fieldValue: any) {
                        const info = fieldValue[0].size <= 204800;
                        return info || "Image is larger that 200KB.";
                    },
                })}
            />
            <p className="text-red-500 flex gap-1 items-center font-bold mb-4 mt-2">
                {errors[name]?.message && <MdErrorOutline />}
                {errors[name]?.message as string}
            </p>

            {isLoading && <p>Loading profile picture...</p>}
        </>
    )
}