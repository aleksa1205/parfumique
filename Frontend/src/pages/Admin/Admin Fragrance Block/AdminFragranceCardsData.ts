import { AdminActionCardDataType } from "../../../components/Admin/AdminActionCardUtils";

export const adminFragranceCardsData : AdminActionCardDataType = [
{ 
    httpMethod: 'GET', 
    endpointAdress: '/fragrance',
    title: 'Get All Fragrances', 
    description: 'Returns all fragrances as array.',
},
{ 
    httpMethod: 'GET', 
    endpointAdress: '/fragrance/without-manufacturer',
    title: 'Get Fragrances without Manufacturer', 
    description: `Returns all fragrances that don't have manufacturer.`, 
},
{
  title: 'Get Fragrance by ID',
  description: 'Returns one fragrance by ID.',
  endpointAdress: '/fragrance',
  httpMethod: 'GET',
  pathParams: ['Id']
},
{ 
    httpMethod: 'POST', 
    endpointAdress: '/fragrance',
    title: 'Add Fragrance', 
    description: 'Adds one fragrance to database.', 
    endpointRequirements: ['Authorization', 'Admin'] ,
    inputExample: `{
    "name": "string",
    "gender": "string",
    "batchYear": 0
}`
},
{ 
    httpMethod: 'PATCH', 
    title: 'Add Notes to Fragrance', 
    endpointAdress: '/fragrance/add-notes',
    description: 'Adds notes to fragrance. Fragrance is found by ID and notes are array.',
    endpointRequirements: ['Authorization', 'Admin'],
    inputExample: `{
  "id": 2147483647,
  "notes": [
    {
      "name": "string",
      "tmb": 2
    }
  ]
}`
},
{ 
    httpMethod: 'PATCH', 
    title: 'Delete Notes from Fragrance', 
    endpointAdress: '/fragrance/delete-notes',
    description: 'Deletes notes from fragrance. Fragrance is found by ID and notes are array.', 
    endpointRequirements: ['Authorization', 'Admin'],
    inputExample: `{
  "id": 2147483647,
  "notes": [
    {
      "name": "string",
      "tmb": 2
    }
  ]
}`
},
{ 
    httpMethod: 'PATCH', 
    title: 'Update Fragrance', 
    endpointAdress: '/fragrance/update',
    description: 'Updates one fragrance by ID.', 
    endpointRequirements: ['Authorization', 'Admin'],
    imageInput: true,
    inputExample: `{
  "id": 2147483647,
  "name": "string",
  "gender": "string",
  "batchYear": 0,
  "image": "string"
}`
},
{ 
    httpMethod: 'DELETE', 
    title: 'Delete Fragrance', 
    endpointAdress: '/fragrance/delete',
    description: 'Deletes fragrance from database.', 
    endpointRequirements: ['Authorization', 'Admin'],
    pathParams: ['Id']
},
]