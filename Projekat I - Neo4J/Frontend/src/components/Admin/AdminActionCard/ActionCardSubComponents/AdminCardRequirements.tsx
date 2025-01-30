import { EndpointRequirements, EndpointRequirementsArray } from "../../AdminActionCardTypes"

export default function Requirements({requirements}:
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