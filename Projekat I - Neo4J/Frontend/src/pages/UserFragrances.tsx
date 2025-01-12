import { useContext, useEffect } from "react";
import FragranceCard from "../components/FragranceCard";
import { CurrUserContext } from "../context/CurrUserProvider";
import { CircleLoader } from "../components/loaders/CircleLoader";
import useUserController from "../api/controllers/useUserController";
import { useInfiniteQuery } from "@tanstack/react-query";
import { useInView } from "react-intersection-observer";
import { Link } from "react-router-dom";

const UserFragrances = () => {
  const { user, isLoading } = useContext(CurrUserContext);
  const { getFragrances } = useUserController();
  const { ref, inView } = useInView();
  const { data, status, fetchNextPage } = useInfiniteQuery(
    //should we do this to prevent getting error pop ups
    ["items", user?.username || ""],
    async ({ queryKey, pageParam = 1 }) => {
      const username = queryKey[1];
      return getFragrances({ username, pageParam });
    },
    {
      getNextPageParam: (lastPage) => lastPage.nextPage,
      enabled: !!user?.username,
    }
  );

  useEffect(() => {
    if (inView) {
      fetchNextPage();
    }
  }, [fetchNextPage, inView]);

  if (isLoading || status === "loading") {
    return (
      <section className="bg-gray-50 antialiased py-12 h-screen flex justify-center items-center">
        <CircleLoader />
      </section>
    );
  }

  return (
    <section className="bg-gray-50 antialiased py-12">
      {" "}
      <div className="mx-auto max-w-screen-xl px-4">
        {user?.collection.length == 0 ? (
          <div className="text-center">
            {" "}
            <h2 className="text-2xl font-bold mb-6 my-text-medium">
              Your collection is empty. Add a fragrance to your collection.
            </h2>
            <Link
              to="/fragrances"
              className="items-center inline-block px-6 py-2.5 text-xl font-semibold bg-white my-text-primary rounded-lg shadow-md hover:bg-gray-200 transition duration-300"
            >
              Fragrances
            </Link>
          </div>
        ) : (
          <>
            <h2 className="text-2xl font-bold mb-6 text-center my-text-medium">
              Your collection
            </h2>
            {data?.pages?.map((page) => (
              <div key={page.currentPage}>
                <div className="mb-4 grid gap-4 sm:grid-cols-2 md:mb-8 lg:grid-cols-3 xl:grid-cols-4 w-full">
                  {page.fragrances.map((fragrance) => (
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
            ))}
            <div ref={ref}></div>
          </>
        )}
      </div>
    </section>
  );
};

export default UserFragrances;
