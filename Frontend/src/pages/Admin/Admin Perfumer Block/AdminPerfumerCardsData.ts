import { AdminActionCardDataType } from "../../../components/Admin/AdminActionCardUtils";

export const adminPerfumerCardsData : AdminActionCardDataType = [
    {
       title: 'Get All Perfumers',
       description: 'Returns all parfumers as array.',
       endpointAdress: '/perfumer',
       httpMethod: 'GET',
    },
    {
        title: 'Get perfumer by ID',
        description: 'Returns perfumer by ID.',
        endpointAdress: '/perfumer',
        httpMethod: 'GET',
        pathParams: ['ID']
    },
    {
        title: 'Add Perfumer',
        description: 'Add one perfumer to database.',
        endpointAdress: '/perfumer',
        httpMethod: 'POST',
        inputExample: `{
  "name": "string",
  "surname": "string",
  "gender": "string",
  "country": "string"
}`,
        endpointRequirements: ['Authorization', 'Admin'],
    },
    {
        title: 'Add Fragrance to Perfumer',
        description: 'Connects fragrance to perfumer.',
        endpointAdress: '/perfumer/add-fragrance-to-perfumer',
        httpMethod: 'PATCH',
        inputExample: `{
  "perfumerId": 2147483647,
  "fragranceId": 2147483647
}`,
        endpointRequirements: ['Authorization', 'Admin'],
    },
    {
        title: 'Update Perfumer',
        description: 'Updates the perfumer.',
        endpointAdress: '/perfumer',
        httpMethod: 'PATCH',
        inputExample: `{
  "id": 2147483647,
  "name": "string",
  "surname": "string",
  "gender": "string",
  "country": "string",
  "image": "string"
}`,
        endpointRequirements: ['Authorization', "Admin"],
        imageInput: true
    },
    {
        title: 'Delete Perfumer',
        description: 'Deletes perfumer from database and all relationships for it.',
        endpointAdress: '/perfumer',
        httpMethod: 'DELETE',
        endpointRequirements: ['Authorization', 'Admin'],
        pathParams: ['ID']
    }
]