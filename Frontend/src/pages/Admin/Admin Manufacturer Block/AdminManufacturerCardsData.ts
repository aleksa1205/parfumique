import { AdminActionCardDataType } from "../../../components/Admin/AdminActionCardUtils";

export const AdminManufacturerCardsData : AdminActionCardDataType = [
    {
        title: "Get All Manufacturers",
        description: 'Returns all fragrances as array.',
        endpointAdress: '/manufacturer',
        httpMethod: 'GET'
    },
    {
        title: "Get Manufacturer By Name",
        description: "Gets one manufacturer by name.",
        httpMethod: 'GET',
        endpointAdress: '/manufacturer',
        pathParams: ['Name']
    },
    {
        title: 'Add Manufacturer',
        description: 'Adds one manufacturer to database.',
        endpointAdress: '/manufacturer',
        httpMethod: 'POST',
        inputExample: `{
  "name": "string"
}`,
        endpointRequirements: ['Authorization', 'Admin']
    },
    {
        title: 'Add Manufacturer to Fragrance',
        description: 'Attaches manufacturer to a given fragrance',
        endpointAdress: '/manufacturer',
        httpMethod: 'PATCH',
        inputExample: `{
  "manufacturerName": "string",
  "fragranceId": 2147483647
}`,
        endpointRequirements: ['Authorization', 'Admin']
    },
    {
        title: 'Update Manufacturer',
        description: 'Update manufacturer by name. Only image can be changed.',
        endpointAdress: '/manufacturer/update',
        httpMethod: 'PATCH',
        inputExample: `{
  "name": "string",
  "image": "string"
}`,
        endpointRequirements: ['Authorization', 'Admin'],
        imageInput: true
    },
    {
        title: 'Delete Manufacturer',
        description: 'Deletes manufacturer from database.',
        endpointAdress: '/manufacturer',
        httpMethod: 'DELETE',
        endpointRequirements: ['Authorization', 'Admin'],
        pathParams: ['Name']
    }
]