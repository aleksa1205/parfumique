import { useEffect, useState } from "react"
import MainButton from "../../../UiComponents/MainButton"
import SecButton from "../../../UiComponents/SecButton"
import { HttpMethods } from "../../AdminActionCardTypes"
import { SpinnerBlade } from "../../../loaders/SpnnerBlade"
import { AxiosResponse, isAxiosError } from "axios"
import useAxiosAuth from "../../../../hooks/useAxiosPrivate"
import { MdErrorOutline } from "react-icons/md"
import { AdminActionCardImageInput } from "./AdminActionCardImageInput"

export default function Input({active, setActiveActionCard, inputValue, setInputValue, httpMethod, endpointAdress, setOutputValue, outputValue, pathParams, imageInput}: {
    active: boolean,
    setActiveActionCard: React.Dispatch<React.SetStateAction<string>>,
    inputValue: string | null,
    setInputValue: React.Dispatch<React.SetStateAction<string | null>>,
    httpMethod: keyof typeof HttpMethods,
    endpointAdress: string,
    setOutputValue: React.Dispatch<React.SetStateAction<string>>,
    outputValue: string,
    pathParams?: string[],
    queryParams?: string[],
    imageInput?: boolean
}) {
    
    const client = useAxiosAuth();

    const [pathParamsValues, setPathParamsValues] = useState<string[]>(new Array(pathParams?.length).fill(''));
    const [isFetching, setIsFetching] = useState<boolean>(false);
    const [imageValue, setImageValue] = useState<string>('');

    useEffect(() => {
        if (isFetching && outputValue)
            setOutputValue("Loading...")
    }, [isFetching])

    useEffect(() => {
        function updateJsonInput() {
            if (inputValue && imageInput) {
                let inputParsed = JSON.parse(inputValue)
                inputParsed["image"] = imageValue
                setInputValue(JSON.stringify(inputParsed, null, 2))
            }
        }

        updateJsonInput();

    }, [imageValue])

    function handleInputChange(event: React.ChangeEvent<HTMLTextAreaElement>) {
        setInputValue(event.target.value)
    }

    function handleInputPathParamsChange(event: React.ChangeEvent<HTMLInputElement>, index: number) {
        setPathParamsValues((prev) => {
            const updatedValues = [...prev]
            updatedValues[index] = event.target.value
            return updatedValues
        })

    }

    async function onClickHandler() {
        try {
            setIsFetching(true);

            let result : AxiosResponse<any, any> | null = null;

            switch (httpMethod) {
                case HttpMethods.GET.label:
                    result = await client.get(endpointAdress);
                    break;
                case HttpMethods.PATCH.label:
                    result = await client.patch(endpointAdress,
                        inputValue,
                        { headers: { 'Content-Type': 'application/json' } }
                    )
                    break;
                case HttpMethods.POST.label:
                    result = await client.post(endpointAdress,
                        inputValue,
                        { headers: { 'Content-Type': 'application/json' } }
                    )
                    break;
                case HttpMethods.PUT.label:
                    result = await client.put(endpointAdress,
                        inputValue,
                        { headers: { 'Content-Type': 'application/json' } }
                    )
                    break;
                case HttpMethods.DELETE.label:
                    if (!pathParams)
                        throw new Error("Delete method must have at least one path parameter. If there is no text input showing up please contact developer");
                    
                    let url = endpointAdress;
                    pathParamsValues.forEach(param => {
                        url += `/${param}`
                    });

                    result = await client.delete(url,
                        { headers: { 'Content-Type': 'application/json' } }
                    )
                    break;
                default:
                    throw new Error(`Unsupported HTTP method:  ${httpMethod}`)
            }      
            
            if (result)
                setOutputValue(JSON.stringify(result.data, null, 2))
            else
                setOutputValue("There was an error while calling server endpoint.");

            setIsFetching(false);
        } catch (error) {
            let errorMessage = 'An unknown error occured.'

            if (isAxiosError(error) && error.response != null)
                    errorMessage = `Status Code: ${error.status}\n\n${JSON.stringify(error.response.data, null, 2)}`
            else if (error instanceof Error)
                errorMessage = error.message 

            setOutputValue(errorMessage)
            setIsFetching(false);
            console.error('Error accessing endpoint from admin panel', error);
        }

    }

    if (!active) return <></>

    return (
        <div className="space-y-4 mt-6">
            {pathParams?.map((item, index) => {
                return (
                    <div
                        key={index}
                        className="w-50"
                    >
                        <label htmlFor="">{item}</label>
                        <input
                            type='text'
                            onChange={(e) => handleInputPathParamsChange(e, index)}
                            className="mt-3 w-full p-2 border rounded-lg transition-all duration-150 hover:ring-2 hover:ring-brand-700 focus:ring-2 focus:ring-brand-500 focus:outline-none"
                        />
                    </div>
                )
            })}

            
            {imageInput && <AdminActionCardImageInput setImageValue={setImageValue} imageValue={imageValue} />}

            {inputValue !== null && (
                <>
                    <label htmlFor="json-input" className="block text-lg font-bold text-gray-700">
                        Input JSON
                    </label>
                    <textarea
                        id="json-input"
                        value={inputValue}
                        onChange={handleInputChange}
                        rows={20}
                        placeholder='e.g., { "key": "value" }'
                        className="w-full p-3 text-sm/4 text-neutral-800 bg-white border-2 border-neutral-300 rounded-lg focus:outline-none focus:border-brand-500 hover:border-brand-700 resize-none"
                    />
                    <p
                        className="text-sm text-red-500 flex items-center gap-x-3 font-bold"
                    >
                        <MdErrorOutline size='1rem' />
                        Watch out for a trailing comma in JSON when writing input.
                        <br />
                        It doesn't show up as an error.
                    </p>
                </>
            )}

            <div className="flex gap-x-4">
                <MainButton 
                    onClick={onClickHandler}
                    disabled={isFetching}
                    >
                        Submit
                    </MainButton>
                <SecButton 
                    disabled={isFetching}
                    onClick={() => {
                        setTimeout(() => {
                            setActiveActionCard('')
                        }, 0)
                }}>Cancel</SecButton>
            </div>

            {isFetching && <SpinnerBlade />}
      </div>
    )
}