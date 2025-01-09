import { isAxiosError, isCancel } from "axios";
import { client } from "../axios";
import { GetUserResponse } from "../../dto-s/responseTypes";

export class WrongCredentials extends Error {
  constructor(message?: string) {
    super(message || "Unauthorized");
    this.name = "Unauthorized";
  }
}

export class UsernameExists extends Error {
  constructor(message?: string) {
    super(message || "Username already in use!");
    this.name = "UsernameExists";
  }
}

export interface User {
  name: string;
  surname: string;
  gender: string;
  username: string;
  password: string;
}

export interface UserLogin {
  username: string;
  password: string;
}

export interface LoginResponse {
  username: string;
  token: string;
}

export default function useUserController() {
  const userController = {
    registerUser: async function (user: User): Promise<void> {
      try {
        await client.post("/User/register", user);
      } catch (error) {
        console.log(error);
        if (isCancel(error)) {
          throw Error("Request was canceled");
        } else if (isAxiosError(error) && error.response != null) {
          switch (error.response.status) {
            case 409:
              throw new UsernameExists();
            default:
              throw Error("Axios Error: " + error.message);
          }
        } else if (error instanceof Error) {
          throw Error("General Error: " + error.message);
        } else {
          throw Error("Unexpected Error: " + error);
        }
      }
    },
    login: async function (user: UserLogin): Promise<LoginResponse> {
      try {
        const response = await client.post(
          `User/login`,
          JSON.stringify({
            Username: user.username,
            Password: user.password,
          }),
          { headers: { "Content-Type": "application/json" } }
        );
        return response.data;
      } catch (error) {
        if (isCancel(error)) {
          throw Error("Request was canceled");
        } else if (isAxiosError(error) && error.response != null) {
          switch (error.response.status) {
            case 401:
              throw new WrongCredentials();
            default:
              throw Error("Axios Error: " + error.message);
          }
        } else if (error instanceof Error) {
          throw Error("General Error: " + error.message);
        } else {
          throw Error("Unexpected Error: " + error);
        }
      }
    },
    get: async function (username: string): Promise<GetUserResponse> {
      try {
        const response = await client.get(`/User/${username}`);
        return response.data;
      } catch (error) {
        if (isAxiosError(error) && error.name === "CanceledError") {
          throw error;
        } else if (error instanceof Error) {
          throw Error("General Error: " + error.message);
        } else {
          throw Error("Unexpected Error: " + error);
        }
      }
    },
  };
  return userController;
}
