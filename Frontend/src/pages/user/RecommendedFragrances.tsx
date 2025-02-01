import { Link, useNavigate } from "react-router-dom";
import useFragranceController from "../../api/controllers/useFragranceController";
import { useQuery } from "@tanstack/react-query";
import { CircleLoader } from "../../components/loaders/CircleLoader";
import FragranceCardAdd from "../../components/FragranceCardAdd";
import MainButton from "../../components/UiComponents/MainButton";

const EmptyRecommendFragrances = () => {
  return (
    <div className="text-center mt-24">
      <h2 className="text-2xl font-bold mb-6 my-text-medium">
        You didn't choose any fragrance!
      </h2>
      <Link
        to="/recommend"
        type="button"
        className="text-white bg-gradient-to-br from-green-400 to-blue-600 
                   hover:bg-gradient-to-bl focus:ring-4 focus:outline-none 
                   focus:ring-green-200 dark:focus:ring-green-800 
                   font-medium rounded-lg text-lg px-8 py-4 text-center"
      >
        Recommend
      </Link>
    </div>
  );
};

const NoReccomendation = () => {
  return (
    <div className="text-center mt-24">
      <h2 className="text-2xl font-bold mb-6 my-text-medium">
        There isn't any fragrance to find by your liking!
      </h2>
      <Link
        to="/user-fragrances"
        type="button"
        className="text-white bg-gradient-to-br from-green-400 to-blue-600 
                       hover:bg-gradient-to-bl focus:ring-4 focus:outline-none 
                       focus:ring-green-200 dark:focus:ring-green-800 
                       font-medium rounded-lg text-lg px-8 py-4 text-center"
      >
        Back
      </Link>
    </div>
  );
};

const RecommendedFragrances = () => {
  const storedFragrances = localStorage.getItem("selectedFragrances");
  const navigate = useNavigate();
  const selectedFragrances = storedFragrances
    ? JSON.parse(storedFragrances)
    : [];
  console.log(selectedFragrances);
  const { recommend } = useFragranceController();

  const { data, isLoading, isFetching } = useQuery(
    ["recommendations", selectedFragrances],
    () => recommend(selectedFragrances),
    {
      keepPreviousData: true,
      onError: (err: Error) => {
        console.log(err.message);
      },
    }
  );

  const onBack = () => {
    localStorage.removeItem("selectedFragrances");
    navigate("/user-fragrances");
  };

  if (isLoading || isFetching) {
    return <CircleLoader />;
  }

  if (selectedFragrances?.length == 0) {
    return <EmptyRecommendFragrances />;
  }

  if (data?.length == 0) {
    localStorage.removeItem("selectedFragrances");
    return <NoReccomendation />;
  }

  const getGridCols = (length: number) => {
    if (length === 1) return "grid-cols-1"; // One item, full width, centered
    if (length === 2) return "grid-cols-2"; // Two items, 2 columns
    if (length === 3) return "grid-cols-3"; // Three items, 3 columns
    if (length === 4) return "grid-cols-4"; // Four items, 4 columns
    return "grid-cols-5"; // Five items, 5 columns
  };

  //change it later to make better ui
  return (
    <section className="bg-gray-50 antialiased py-12 mt-16">
      <div className="mx-auto max-w-screen-xl px-4">
        <h1 className="text-brand-500 text-3xl font-semibold mb-6 text-center">
          Recommended Fragrances
        </h1>
        <div
          className={`mb-4 grid gap-4 w-full ${getGridCols(data!.length)} 
                      ${data!.length === 1 ? "justify-center" : ""}`} // Center the single item
        >
          {" "}
          {data?.map((fragrance) => (
            <div
              key={fragrance.id}
              className="rounded-lg border border-gray-200 bg-white p-6 shadow-sm"
            >
              <FragranceCardAdd {...fragrance} />
            </div>
          ))}
        </div>
      </div>
      <div className="flex justify-center mt-8">
        <MainButton onClick={onBack}>Back</MainButton>
      </div>
    </section>
  );
};

export default RecommendedFragrances;
