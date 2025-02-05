import { useQuery } from "@tanstack/react-query";
import useManufacturerController from "../api/controllers/useManufacturerController";
import { useParams } from "react-router-dom";
import { CircleLoader } from "../components/loaders/CircleLoader";
import { base64ToUrl } from "../utils";
import FragranceCard from "../components/FragranceCard";

const Manufacturer = () => {
  const { name } = useParams();
  const { get } = useManufacturerController();
  const { data, isLoading, isFetching } = useQuery(
    ["manufacturer", name],
    () => get(name || ""),
    {
      enabled: !!name,
      onError: (err: Error) => {
        console.log(err.message);
      },
    }
  );

  if (isLoading || isFetching) {
    return <CircleLoader />;
  }

  return (
    <div className="flex flex-col items-center justify-center px-6 py-8 mx-auto font-roboto mt-16">
      <h1 className="text-4xl font-extrabold text-gray-800 mb-4 tracking-wide">
        {data?.name}
      </h1>{" "}
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
        <span className="font-semibold text-gray-800">
          {data?.fragrances.length}
        </span>
      </p>{" "}
      {/* Fragrances Grid */}
      <div
        className={`
    max-w-6xl mx-auto px-4 py-8 grid gap-6
    ${data?.fragrances.length < 3 ? "place-items-center" : ""}
    grid-cols-${Math.min(data?.fragrances.length, 4)}
  `}
      >
        {" "}
        {data?.fragrances?.map((fragrance) => (
          <FragranceCard {...fragrance} />
        ))}
      </div>
    </div>
  );
};

export default Manufacturer;
