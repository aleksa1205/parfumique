import { useParams } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import useFragranceController from "../api/controllers/useFragranceController";

const FragranceDetails = () => {
  const { id } = useParams();
  const fragranceId = parseInt(id ?? "", 10);
  const { get } = useFragranceController();
  const {
    data: fragrance,
    isLoading,
    isError,
    error,
  } = useQuery(["fragrance", fragranceId], () => get(fragranceId), {
    onError: (err: Error) => console.log(err.message),
    enabled: !isNaN(fragranceId),
  });
  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (isError) {
    return <div>Error... {error.message}</div>;
  }

  return (
    <div className="flex flex-col items-ceneter justify-center px-6 py-8 mx-auto">
      <div className="w-full text-center font-roboto">
        <h1 className="text-3xl font-bold mb-2">
          {fragrance.name} for {fragrance.gender}
        </h1>
        <div className="my-text-black">
          <Link
            to={`/manufacturers/${fragrance.manufacturer.name}`}
            className="text-lg font-bold my-text-primary"
          >
            {fragrance.manufacturer.name}
          </Link>
          <p className="text-lg">Batch Year: {fragrance.batchYear}</p>
        </div>
      </div>
    </div>
  );
};

export default FragranceDetails;

/*{
  /*onError: (error: AxiosError) => {
        return error.message;
      },*/
