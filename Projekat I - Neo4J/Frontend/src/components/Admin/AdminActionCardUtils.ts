import { EndpointRequirementsArray, HttpMethods } from "./AdminActionCardTypes";

type HttpMethodWithBody = "POST" | "PUT" | "PATCH";
type HttpMethodWithoutBody = "GET" | "DELETE";

type BaseAdminActionCardDataType = {
    title: string,
    httpMethod: keyof typeof HttpMethods,
    endpointAdress: string,
    description: string,
    endpointRequirements?: EndpointRequirementsArray,
    imageInput?: boolean
}

export type AdminActionCardDataTypeWithBody = BaseAdminActionCardDataType & {
    httpMethod: HttpMethodWithBody;
    inputExample: string,
}

export type AdminActionCardDataTypeWithoutBody = BaseAdminActionCardDataType & {
    httpMethod: HttpMethodWithoutBody;
    queryParams?: string[]; // Example: ?page=1&limit=10
    pathParams?: string[];  // Example: /users/123
}

export type AdminActionCardDataType = (AdminActionCardDataTypeWithoutBody | AdminActionCardDataTypeWithBody)[];

export function hasBody(
    item: AdminActionCardDataTypeWithoutBody | AdminActionCardDataTypeWithBody
): item is AdminActionCardDataTypeWithBody {
    return ["POST", "PUT", "PATCH"].includes(item.httpMethod);
}