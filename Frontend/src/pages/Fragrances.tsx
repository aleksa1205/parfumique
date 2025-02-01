import { useQuery } from "@tanstack/react-query";
import useFragranceController from "../api/controllers/useFragranceController";
import { CircleLoader } from "../components/loaders/CircleLoader";
import { useEffect, useState } from "react";
import Pagination from "../components/Pagination";
import FragranceCardAdd from "../components/FragranceCardAdd";
import useIsLoggedIn from "../hooks/useIsLoggedIn";
import FragranceCard from "../components/FragranceCard";
import usePopUpMessage from "../hooks/usePopUpMessage";
import UseAuth from "../hooks/useAuth";

const Fragrances = () => {
  const [page, setPage] = useState(1);
  const { get } = useFragranceController();
  const isLoggedIn = useIsLoggedIn();
  const { auth } = UseAuth();

  const { PopUpComponent, setPopUpMessage } = usePopUpMessage();

  const {
    data: response,
    isLoading,
    isFetching,
    isPreviousData,
  } = useQuery(["response", page], () => get(page), {
    keepPreviousData: true,
    onError: (err: Error) => {
      console.log(err.message);
    },
  });

  useEffect(() => {
    setTimeout(() => {
      if (auth.jwtToken === "") {
        setPopUpMessage({
          message: "You need to be logged in in order to add fragrances to your collection.",
          type: 'info',
          dontClose: true
        })
      }
    }, 1000)
  }, [isFetching, isLoading, auth])

  if (isLoading || isFetching) {
    return <CircleLoader />;
  }
  return (
    <>
    <PopUpComponent />
    <section className="bg-gray-50 antialiased py-12 mt-16">
      <div className="mx-auto max-w-screen-xl px-4">
        <div className="mb-4 grid gap-4 sm:grid-cols-2 md:mb-8 lg:grid-cols-3 xl:grid-cols-4 w-full">
          {response?.fragrances.map((fragrance) => (
            <div
              key={fragrance.id}
              className="rounded-lg border border-gray-200 bg-white p-6 shadow-sm "
            >
              {isLoggedIn ? (
                <FragranceCardAdd {...fragrance} />
              ) : (
                <FragranceCard {...fragrance} />
              )}
            </div>
          ))}
        </div>
        <Pagination
          page={page}
          setPage={setPage}
          numberOfPages={response?.totalPages as number}
          isPreviousData={isPreviousData}
        />
      </div>
    </section>
    </>
  );
};

export default Fragrances;
