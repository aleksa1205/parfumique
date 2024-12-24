import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { Loader } from "../components/loaders/Loader";
import { base64ToUrl } from "../utils";
import { NotFoundError } from "../api/controllers/useFragranceController";
import useFragranceController from "../api/controllers/useFragranceController";

const FragranceDetails = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const fragranceId = parseInt(id ?? "", 10);
  const [notFound, setNotFound] = useState(false);
  const { getById } = useFragranceController();
  const {
    data: fragrance,
    isLoading,
    isError,
    error,
  } = useQuery(["fragrance", fragranceId], () => getById(fragranceId), {
    onError: (err: Error) => {
      if (err instanceof NotFoundError) {
        setNotFound(true);
      }
      console.log(err.message);
    },
    enabled: !isNaN(fragranceId),
  });

  const perfumers = fragrance?.perfumers.slice(0, 5);
  const top = fragrance?.top;
  const middle = fragrance?.middle;
  const base = fragrance?.base;

  useEffect(() => {
    if (notFound) {
      navigate(`/not-found`);
    }
  }, [notFound, navigate]);

  if (isError) {
    return <div>Error... {error.message}</div>;
  }
  return (
    <div className="flex flex-col items-center justify-center px-6 py-8 mx-auto font-roboto">
      {isLoading && <Loader />}
      {!isLoading && !isError && fragrance && (
        <div className="w-full max-w-screen-xl text-center">
          {fragrance.image && (
            <img
              src={base64ToUrl(fragrance.image)}
              className="w-64 h-64 rounded-xl mx-auto"
            />
          )}

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
            <hr className="y-6 border-gray-200 mx-auto my-3" />
            {/* Perfumers display */}
            <h3 className="text-xl font-bold mb-2">Perfumers</h3>
            <div className="flex justify-evenly gap-4">
              {perfumers?.map((perfumer) => (
                <div key={perfumer.id} className="flex flex-col items-center">
                  <img
                    src={base64ToUrl(perfumer.image)}
                    alt={`${perfumer.name} ${perfumer.surname} image`}
                    className="h-24 w-24 rounded-full"
                  />
                  <Link
                    to={`/perfumers/${perfumer.id}`}
                    className="text-lg mt-4 my-text-primary"
                  >
                    {perfumer.name} {perfumer.surname}
                  </Link>
                </div>
              ))}
            </div>
            <hr className="y-6 border-gray-200 mx-auto my-3" />
            {/* Notes pyramid */}
            <h3 className="text-xl font-bold mb-2">Fragrance pyramid</h3>
            <h2 className="text-lg mb-2">Top notes</h2>
            <div className="flex justify-center gap-3">
              {top?.map((note) => (
                <div key={note.name} className="flex flex-col">
                  <img
                    src={base64ToUrl(note.image)}
                    alt={`${note.name} image`}
                    className="h-12 w-12"
                  />
                  {note.name}
                </div>
              ))}
            </div>
            <h2 className="text-lg m-2">Middle notes</h2>
            <div className="flex justify-center gap-3">
              {middle?.map((note) => (
                <div key={note.name} className="flex flex-col">
                  <img
                    src={base64ToUrl(note.image)}
                    alt={`${note.name} image`}
                    className="h-12 w-12"
                  />
                  {note.name}
                </div>
              ))}
            </div>
            <h2 className="text-lg m-2">Base notes</h2>
            <div className="flex justify-center gap-3">
              {base?.map((note) => (
                <div key={note.name} className="flex flex-col">
                  <img
                    src={base64ToUrl(note.image)}
                    alt={`${note.name} image`}
                    className="h-12 w-12"
                  />
                  {note.name}
                </div>
              ))}
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default FragranceDetails;
