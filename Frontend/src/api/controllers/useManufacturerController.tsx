import { isAxiosError } from "axios";
import { Manufacturer } from "../../dto-s/ManufacturerDto";
import { client } from "../axios";
import { NotFoundError } from "../../dto-s/Errors";

export default function useManufacturerController() {
  const ManufacturerController = {
    get: async function (name: string): Promise<Manufacturer> {
      try {
        const response = await client.get<Manufacturer>(
          `/Manufacturer/${name}`
        );
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
  return ManufacturerController;
}
