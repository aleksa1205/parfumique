import { useQuery } from "@tanstack/react-query";
import FragranceCard from "../components/FragranceCard";
import useFragranceController from "../api/controllers/useFragranceController";
import { CircleLoader } from "../components/loaders/CircleLoader";

const Fragrances = () => {
  const { get } = useFragranceController();
  const { data: fragrances, isLoading } = useQuery(
    ["fragrances"],
    () => get(),
    {
      onError: (err: Error) => {
        console.log(err.message);
      },
    }
  );
  if (isLoading) {
    return (
      <section className="bg-gray-50 antialiased py-12 h-screen flex justify-center items-center">
        <CircleLoader />
      </section>
    );
  }
  {
    /*console.log(fragrances);*/
  }
  return (
    <section className="bg-gray-50 antialiased py-12">
      <div className="mx-auto max-w-screen-xl px-4">
        <div className="mb-4 grid gap-4 sm:grid-cols-2 md:mb-8 lg:grid-cols-3 xl:grid-cols-4 w-full">
          {fragrances?.map((fragrance) => (
            <div
              key={fragrance.id}
              className="rounded-lg border border-gray-200 bg-white p-6 shadow-sm "
            >
              <FragranceCard
                id={fragrance.id.toString()}
                image={fragrance.image ? fragrance.image : ""}
                name={fragrance.name}
                gender={fragrance.gender}
              />
            </div>
          ))}
        </div>
      </div>
    </section>
  );
};

export default Fragrances;
