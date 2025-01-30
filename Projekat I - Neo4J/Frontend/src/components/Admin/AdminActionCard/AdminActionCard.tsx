import { useEffect, useState } from "react"
import Description from "./ActionCardSubComponents/AdminCardDescription"
import FirstBlock from "./ActionCardSubComponents/AdminCardFirstBlock"
import Input from "./ActionCardSubComponents/AdminCardInput"
import Output from "./ActionCardSubComponents/AdminCardOutput"
import Requirements from "./ActionCardSubComponents/AdminCardRequirements"
import { AdminActionCardDataTypeWithBody, AdminActionCardDataTypeWithoutBody, hasBody } from "../AdminActionCardUtils"

type AdminActionCardPropType = {
    data: (AdminActionCardDataTypeWithoutBody | AdminActionCardDataTypeWithBody),
    active: boolean,
    setActiveActionCard: React.Dispatch<React.SetStateAction<string>>,
}

export default function AdminActionCard({data, active, setActiveActionCard}: AdminActionCardPropType) {
    const { description, endpointAdress, httpMethod, title, endpointRequirements, imageInput } = data;

    const [inputValue, setInputValue] = useState<string | null>(null);
    const [outputValue, setOutputValue] = useState<string>('')
    const [pathParams, setPathParams] = useState<string[] | undefined>(undefined)
    const [queryParams, setQueryParams] = useState<string[] | undefined>(undefined);

    useEffect(() => {
        if(hasBody(data)) {
            const { inputExample } = data
            setInputValue(inputExample)
        }
        else {
            const { pathParams: pp, queryParams: qp } = data
            setPathParams(pp);
            setQueryParams(qp);
        }
    }, [])
    
    
    return (
        <div 
            className={`group bg-neutral-50 px-8 py-5 w-full cursor-pointer transition ease-in-out
                        duration-100 rounded-md hover:bg-neutral-100 ${active ? 'outline outline-2 outline-brand-500': ''}`}
            onClick={() => setActiveActionCard(title)}
        >
            <FirstBlock httpMethod={httpMethod} title={title} />
            <Description description={description} />
            <Requirements requirements={endpointRequirements}/>
            <Input 
                endpointAdress={endpointAdress} 
                httpMethod={httpMethod} 
                active={active} 
                setActiveActionCard={setActiveActionCard} 
                inputValue={inputValue} 
                outputValue={outputValue} 
                setInputValue={setInputValue} 
                setOutputValue={setOutputValue}
                pathParams={pathParams}
                queryParams={queryParams}
                imageInput={imageInput}
            />
            <Output outputValue={outputValue} active={active} />
        </div>
    )
}