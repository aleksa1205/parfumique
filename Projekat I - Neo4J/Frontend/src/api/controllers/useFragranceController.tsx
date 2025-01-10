import { isAxiosError } from "axios";
import { client } from "../axios";
import { NotFoundError } from "../../dto-s/Errors";
import { Fragrance, FragrancePagination } from "../../dto-s/FragranceDto";

export default function useFragranceController() {
  const FragranceController = {
    get: async function (page: number): Promise<FragrancePagination> {
      try {
        const response = await client.get<FragrancePagination>(
          `/Fragrance/${page}/${8}`
        );
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
