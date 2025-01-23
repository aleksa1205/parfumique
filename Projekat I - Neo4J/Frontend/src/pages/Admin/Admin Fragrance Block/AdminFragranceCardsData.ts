import { AdminActionCardDataType } from "../../../components/Admin/AdminActionCardContainer";

export const adminFragranceCardsData : AdminActionCardDataType = [
{ 
    httpMethod: 'GET', 
    title: 'Get All Fragrances', 
    description: 'Returns all fragrances as array.', 
},
{ 
    httpMethod: 'GET', 
    title: 'Get Fragrances without Manufacturer', 
    description: `Returns all fragrances that don't have manufacturer.`, 
},
{ 
    httpMethod: 'POST', 
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
    description: 'Updates one fragrance by ID.', 
    endpointRequirements: ['Authorization', 'Admin'],
    inputExample: `{
  "id": 2147483647,
  "name": "string",
  "gender": "string",
  "batchYear": 0
}`
},
{ 
    httpMethod: 'DELETE', 
    title: 'Delete Fragrance', 
    description: 'Deletes fragrance from database.', 
    endpointRequirements: ['Authorization', 'Admin'],
    inputExample: `{
  "id": 2147483647
}`
},
]