import { AdminActionCardDataType } from "../../../components/Admin/AdminActionCardUtils";

export const adminNoteCardsData : AdminActionCardDataType = [
    {
        title: "Get All Notes",
        description: "Returns all notes as array.",
        endpointAdress: "/note",
        httpMethod: 'GET',
    },
    {
        title: "Get Note by Name",
        description: "Returns one note by searching it by name.",
        endpointAdress: '/note',
        httpMethod: 'GET',
        pathParams: ['Name']
    },
    {
        title: "Create Note",
        description: "Adds one note to the database.",
        endpointAdress: '/note',
        httpMethod: 'POST',
        inputExample: `{
  "name": "string",
  "type": "string"
}`,
        endpointRequirements: ['Authorization', 'Admin']
    },
    {
        title: "Update Note",
        description: "Updates one note by searching its name.",
        endpointAdress: '/note',
        httpMethod: 'PATCH',
        inputExample: `{
  "name": "string",
  "type": "string",
  "image": "string"
}`,
        endpointRequirements: ['Authorization', 'Admin'],
        imageInput: true
    },
    {
        title: "Delete Note",
        description: "Deletes one note by seraching its name.",
        endpointAdress: '/note',
        httpMethod: 'DELETE',
        endpointRequirements: ['Authorization', 'Admin'],
        pathParams: ['Name']
    }
]