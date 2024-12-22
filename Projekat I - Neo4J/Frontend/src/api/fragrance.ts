import { useQuery } from "@tanstack/react-query";

const url = "https://localhost:8080/Fragrance";
interface Fragrance {
  id: number;
  name: string;
  batchYear: number;
  gender: string;
}

const useGetFragrance = (fragranceId: number):  => {
  return useQuery({
    queryKey: ["fragrance", { fragranceId }],
    queryFn: async () => {
      const response = await fetch(`${url}/${fragranceId}`);
      return (await response.json()) as Fragrance;
    },
  });
};

export default useGetFragrance;
