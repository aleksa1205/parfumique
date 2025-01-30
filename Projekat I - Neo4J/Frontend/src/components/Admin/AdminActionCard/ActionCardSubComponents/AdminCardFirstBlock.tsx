import { HttpMethods } from "../../AdminActionCardTypes"

export default function FirstBlock(
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