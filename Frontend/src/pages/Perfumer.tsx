import { useQuery } from "@tanstack/react-query";
import { useParams } from "react-router-dom";
import { CircleLoader } from "../components/loaders/CircleLoader";
import { base64ToUrl } from "../utils";
import usePerfumerController from "../api/controllers/usePerfumerController";
import FragranceCard from "../components/FragranceCard";

const Perfumer = () => {
  const { id } = useParams();
  const perfumerId = parseInt(id ?? "", 10);
  const { get } = usePerfumerController();
  const { data, isLoading, isFetching } = useQuery(
    ["perfumer", perfumerId],
    () => get(perfumerId),
    {
      onError: (err: Error) => {
        console.log(err.message);
      },
    }
  );

  if (isLoading || isFetching) {
    return <CircleLoader />;
  }

  const fragranceCount = data?.fragrances.length ?? 0;

  return (
    <div className="flex flex-col items-center justify-center px-6 py-8 mx-auto font-roboto mt-16">
      <h1 className="text-4xl font-extrabold text-gray-800 mb-4 tracking-wide">
        {data?.name} {data?.surname}
      </h1>
      {data?.image && (
        <div className="w-60 h-60 overflow-hidden rounded-2xl shadow-lg transition-transform duration-300 hover:scale-105">
          <img
            alt={`${data?.name} image`}
            src={base64ToUrl(data!.image)}
            className="w-full h-full object-cover"
          />
        </div>
      )}
      <p className="mt-4 text-lg text-gray-600">
        Total Fragrances in Database:{" "}
        <span className="font-semibold text-gray-800">{fragranceCount}</span>
      </p>{" "}
      <div
        className={`
    max-w-6xl mx-auto px-4 py-8 grid gap-6
    ${fragranceCount < 3 ? "place-items-center" : ""}
    grid-cols-${Math.min(fragranceCount, 4)}
  `}
      >
        {data?.fragrances?.map((fragrance) => (
          <FragranceCard key={fragrance.id} {...fragrance} />
        ))}
      </div>
    </div>
  );
};

export default Perfumer;
