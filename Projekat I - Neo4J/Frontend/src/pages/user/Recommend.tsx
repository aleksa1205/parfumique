import { useInfiniteQuery } from "@tanstack/react-query";
import { useContext, useEffect, useState } from "react";
import { useInView } from "react-intersection-observer";
import useUserController from "../../api/controllers/useUserController";
import { CurrUserContext } from "../../context/CurrUserProvider";
import { CircleLoader } from "../../components/loaders/CircleLoader";
import { Loader } from "../../components/loaders/Loader";
import SelectableFragranceCard from "../../components/SelectableFragranceCard";

const Recommend = () => {
  const [selectedFragrances, setSelectedFragrances] = useState<Set<string>>(
    new Set()
  );
  const { user, isLoading } = useContext(CurrUserContext);
  const { getFragrances } = useUserController();
  const { ref, inView } = useInView();
  const { data, status, fetchNextPage, isFetchingNextPage, isFetching } =
    useInfiniteQuery(
      //should we do this to prevent getting error pop ups
      ["items", user?.username || ""],
      async ({ queryKey, pageParam = 1 }) => {
        const username = queryKey[1];
        return await getFragrances({ username, pageParam });
      },
      {
        getNextPageParam: (lastPage) => lastPage.nextPage,
        enabled: !!user?.username,
      }
    );

  useEffect(() => {
    if (inView && data?.pages[data.pages.length - 1].nextPage) {
      fetchNextPage();
    }
  }, [fetchNextPage, inView, data]);

  const handleSelect = (id: string, selected: boolean) => {
    setSelectedFragrances((prev) => {
      const newSelection = new Set(prev);
      if (selected) {
        if (newSelection.size >= 3) {
          //pop up
          return prev;
        }
        newSelection.add(id);
      } else {
        newSelection.delete(id);
      }
      return newSelection;
    });
  };

  if (isLoading || status === "loading" || isFetching) {
    return <CircleLoader />;
  }
  return (
    <section className="bg-gray-50 min-h-screen py-10 mt-24">
      <div className="container mx-auto px-4 max-w-screen-xl">
        <h2 className="text-2xl font-bold mb-6 text-center text-brand-500">
          Recommend
        </h2>

        <div className="grid gap-4 grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-4 w-full">
          {data?.pages?.map((page) =>
            page.fragrances.map((fragrance) => (
              <div
                key={fragrance.id}
                className="w-full max-w-xs mx-auto p-4 shadow-md"
              >
                <SelectableFragranceCard
                  {...fragrance}
                  id={String(fragrance.id)}
                  selected={selectedFragrances.has(String(fragrance.id))}
                  onSelect={handleSelect}
                />
              </div>
            ))
          )}
        </div>

        {isFetchingNextPage && (
          <section className="w-full flex justify-center items-center py-4">
            <Loader />
          </section>
        )}

        <div ref={ref}></div>
      </div>
    </section>
  );
};

export default Recommend;
