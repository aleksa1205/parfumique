import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { Loader } from "../components/loaders/Loader";
import { base64ToUrl } from "../utils";
import useFragranceController from "../api/controllers/useFragranceController";
import { NotFoundError } from "../dto-s/Errors";

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
  const manufacturer = fragrance?.manufacturer;
  const top = fragrance?.top;
  const middle = fragrance?.middle;
  const base = fragrance?.base;

  useEffect(() => {
    if (notFound) {
      navigate(`/not-found`);
    }
  }, [notFound, navigate]);

  if (isError) {
    return <div className="mt-16">Error... {error.message}</div>;
  }

  return (
    <div className="flex flex-col items-center justify-center px-6 py-8 mx-auto font-roboto mt-16">
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
            {manufacturer && (
              <Link
                to={`/manufacturers/${fragrance.manufacturer.name}`}
                className="text-lg font-bold my-text-primary"
              >
                {fragrance.manufacturer.name}
              </Link>
            )}
            <p className="text-lg">Batch Year: {fragrance.batchYear}</p>
            <hr className="y-6 border-gray-200 mx-auto my-3" />
            {/* Perfumers display */}
            <h3 className="text-2xl font-bold mb-4">Perfumers</h3>
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
            <h3 className="text-2xl font-bold mb-8">Fragrance pyramid</h3>
            <h2 className="text-lg mb-5 font-bold">Top notes</h2>
            <div className="flex justify-center gap-3">
              {top?.map((note) => (
                <div key={note.name} className="flex flex-col items-center justify-center">
                  <img
                    src={base64ToUrl(note.image)}
                    alt={`image`}
                    className="h-12 w-12"
                  />
                  {note.name}
                </div>
              ))}
            </div>
            <h2 className="text-lg mt-3 mb-5 font-bold">Middle notes</h2>
            <div className="flex justify-center gap-3">
              {middle?.map((note) => (
                <div key={note.name} className="flex flex-col items-center justify-center">
                  <img
                    src={base64ToUrl(note.image)}
                    alt={`image`}
                    className="h-12 w-12"
                  />
                  {note.name}
                </div>
              ))}
            </div>
            <h2 className="text-lg mt-3 mb-5 font-bold">Base notes</h2>
            <div className="flex justify-center gap-3">
              {base?.map((note) => (
                <div key={note.name} className="flex flex-col items-center justify-center">
                  <img
                    src={base64ToUrl(note.image)}
                    alt={`image`}
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
