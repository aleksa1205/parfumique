import { useState } from "react";
import AdminActionCard from "./AdminActionCard";
import { EndpointRequirementsArray, HttpMethods } from "./AdminActionCardTypes"

export type AdminActionCardDataType = {
    title: string,
    httpMethod: keyof typeof HttpMethods,
    description: string,
    endpointRequirements?: EndpointRequirementsArray,
    inputExample?: string,
}[];


export function AdminActionCardContainer({data}:
     {data: AdminActionCardDataType}) 
{
    const [activeActionCard, setActiveActionCard] = useState<string>('');

    return (
        <main className="max-w-screen-xl mx-auto px-4 mt-10">
            
            <div className="grid grid-cols-1 gap-x-5 gap-y-10">
            {data.map(item => {
                return (
                    <AdminActionCard
                        key={item.title}
                        httpMethod={item.httpMethod}
                        title={item.title}
                        description={item.description}
                        endpointRequirements={item.endpointRequirements}
                        active={activeActionCard === item.title}
                        setActiveActionCard={setActiveActionCard}
                        inputExample={item.inputExample}
                    >

                    </AdminActionCard>
                )
            })}
            </div>
        </main>
    )
}