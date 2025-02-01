import { AdminActionCardDataType } from "../../../components/Admin/AdminActionCardUtils";

export const adminUserCardsData : AdminActionCardDataType = [
    {
        title: "Get All Users",
        description: "Returns all users as array.",
        endpointAdress: "/user",
        httpMethod: "GET",
    },
    {
        title: "Get User by Username",
        description: "Returns one user by searching its username.",
        endpointAdress: "/user",
        httpMethod: 'GET',
        endpointRequirements: ['Authorization', "Admin"],
        pathParams: ["Username"]
    },
    {
        title: "Update User",
        description: "Updates a user by searching its username.",
        endpointAdress: "user/update-user",
        httpMethod: "PATCH",
        inputExample: `{
  "username": "string",
  "name": "string",
  "surname": "string",
  "gender": "string",
  "image": "string"
}`,
        endpointRequirements: ['Authorization', "Admin"],
        imageInput: true
    },
    {
        title: "Add Fragrance to User",
        description: "Adds one fragrance to user.",
        endpointAdress: "/user/add-fragrance-to-user",
        httpMethod: "PATCH",
        inputExample: `{
  "username": "string",
  "fragranceId": 2147483647
}`,     
        endpointRequirements: ['Authorization', 'Admin']
    },
    {
        title: "Delete User",
        description: "Deletes one user from database.",
        endpointAdress: "/user/delete-user",
        httpMethod: 'DELETE',
        endpointRequirements: ['Authorization', 'Admin'],
        pathParams: ["Username"]
    }
]