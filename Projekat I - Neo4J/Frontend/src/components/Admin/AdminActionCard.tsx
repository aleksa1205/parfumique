import MainButton from "../UiComponents/MainButton"
import SecButton from "../UiComponents/SecButton"
import { EndpointRequirements, EndpointRequirementsArray, HttpMethods } from "./AdminActionCardTypes"

type AdminActionCardPropType = {
    title: string,
    httpMethod: keyof typeof HttpMethods,
    description: string,
    endpointRequirements?: EndpointRequirementsArray,
    active: boolean,
    setActiveActionCard: React.Dispatch<React.SetStateAction<string>>,
    inputExample?: string
}

export default function AdminActionCard({title, httpMethod, description, endpointRequirements, active, setActiveActionCard, inputExample}: AdminActionCardPropType) {
    return (
        <div 
            className={`group bg-neutral-50 px-8 py-5 w-full cursor-pointer transition ease-in-out
                        duration-100 rounded-md hover:bg-neutral-100 ${active ? 'outline outline-2 outline-brand-500': ''}`}
            onClick={() => setActiveActionCard(title)}
        >
            <FirstBlock httpMethod={httpMethod} title={title} />
            <Description description={description} />
            <Requirements requirements={endpointRequirements}/>
            <Input active={active} setActiveActionCard={setActiveActionCard} inputExample={inputExample} />
            <Output />
        </div>
    )
}




function FirstBlock(
    {title, httpMethod} : {
        title: string,
        httpMethod: keyof typeof HttpMethods
    }
) {
    const { label, className } = HttpMethods[httpMethod]
    return (
        <div className="flex gap-x-2 justify-between items-center mb-12">
            <h2 className="text-4xl font-bold">
                {title}
            </h2>

            <div className={`py-2 px-4 ${className} rounded-md font-semibold transition ease-in-out duration-100`}>
                {label}
            </div>
        </div>
    )
}

function Description({description} : {description: string}) {
    return (
        <p className="text-neutral-400 mb-4 font-light">
            {description}
        </p>
    )
}

function Requirements({requirements}:
    {requirements?: EndpointRequirementsArray}
) {
    return (
        <div className="flex gap-x-4 items-center">
            {requirements && (
                <span className="text-neutral-400 font-light">Requirements: </span>
            )}
            <div className="flex gap-x-3">
                {requirements?.map(item => {
                    const { label, className } = EndpointRequirements[item]

                    return (
                        <div
                            key={item}
                            className={`${className} border-2 px-2 py-1 rounded-md transition ease-in-out duration-100`}
                        >
                            {label}
                        </div>
                    )
                })}
            </div>
        </div>
    )
}

function Input({active, setActiveActionCard, inputExample}: {
    active: boolean,
    setActiveActionCard: React.Dispatch<React.SetStateAction<string>>,
    inputExample?: string
}) {

    if (!active) return <></>

    return (
        <div className="space-y-4 mt-6">
            {inputExample && (
                <>
                    <label htmlFor="json-input" className="block text-lg font-bold text-gray-700">
                        Input JSON
                    </label>
                    <textarea
                        id="json-input"
                        value={inputExample}
                        //   onChange={handleChange}
                        rows={8}
                        placeholder='e.g., { "key": "value" }'
                        className="w-full p-3 text-sm text-neutral-800 bg-white border-2 border-neutral-300 rounded-lg focus:outline-none focus:border-brand-500 hover:border-brand-700 resize-none"
                    ></textarea>
                </>
            )}

            <div className="flex gap-x-4">
                <MainButton onClick={() => {}}>Submit</MainButton>
                <SecButton onClick={() => {
                    setTimeout(() => {
                        setActiveActionCard('')
                    }, 0)
                }}>Cancel</SecButton>
            </div>
      </div>
    )
}

function Output() {
    return (
        <div>
            <p></p>
        </div>
    )
}