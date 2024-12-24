import { isAxiosError } from "axios";
import { client } from "../axios";

export class NotFoundError extends Error {
  constructor(message?: string) {
    super(message || "Not Found");
    this.name = "NotFound";
  }
}

interface Fragrance {
  id: number;
  image: string;
  name: string;
  batchYear: number;
  gender: string;
  manufacturer: {
    name: string;
  };
  perfumers: {
    id: number;
    name: string;
    surname: string;
    image: string;
  }[];
  top: {
    image: string;
    name: string;
  }[];
  middle: {
    image: string;
    name: string;
  }[];
  base: {
    image: string;
    name: string;
  }[];
}

interface ApiResponse {
  page: number;
  size: number;
  totalPages: number;
  listOfFragrances: Array<{
    id: number;
    properties: Omit<Fragrance, "id">;
  }>;
}

export default function useFragranceController() {
  const FragranceController = {
    get: async function (page: number): Promise<ApiResponse> {
      try {
        const response = await client.get<ApiResponse>(
          `/Fragrance?pageNumber=${page}&pageSize=${6}`
        );
        console.log(response.data);
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
    getById: async function (id: number): Promise<Fragrance> {
      try {
        const response = await client.get(`/Fragrance/${id}`);
        return response.data;
      } catch (error) {
        if (isAxiosError(error) && error.name === "CanceledError") {
          throw error;
        } else if (isAxiosError(error) && error.response != null) {
          switch (error.response.status) {
            case 404:
              throw new NotFoundError();
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
  };
  return FragranceController;
}
