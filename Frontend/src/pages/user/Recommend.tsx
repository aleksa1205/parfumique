import { useInfiniteQuery } from "@tanstack/react-query";
import { useContext, useEffect, useState } from "react";
import { useInView } from "react-intersection-observer";
import useUserController from "../../api/controllers/useUserController";
import { CurrUserContext } from "../../context/CurrUserProvider";
import { CircleLoader } from "../../components/loaders/CircleLoader";
import { Loader } from "../../components/loaders/Loader";
import SelectableFragranceCard from "../../components/SelectableFragranceCard";
import { useNavigate } from "react-router-dom";
import usePopUpMessage from "../../hooks/Animated Components/usePopUpMessage";

const Recommend = () => {
  const [selectedFragrances, setSelectedFragrances] = useState<Set<number>>(
    new Set()
  );
  const { user, isLoading } = useContext(CurrUserContext);
  const { getFragrances } = useUserController();
  const { ref, inView } = useInView();
  const navigate = useNavigate();
  const { setPopUpMessage, PopUpComponent } = usePopUpMessage();
  const { data, status, fetchNextPage, isFetchingNextPage } = useInfiniteQuery(
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

  useEffect(() => {
    const storedFragrances = localStorage.getItem("selectedFragrances");
    if (storedFragrances) {
      setSelectedFragrances(new Set(JSON.parse(storedFragrances)));
    }
  }, []);

  const handleSelect = (id: number, selected: boolean) => {
    setSelectedFragrances((prev) => {
      const newSelection = new Set(prev);
      if (selected) {
        if (newSelection.size >= 3) {
          setPopUpMessage({
            type: "warning",
            message: "You can't select more than 3 fragrances!",
          });
          return prev;
        }
        newSelection.add(id);
      } else {
        newSelection.delete(id);
      }
      localStorage.setItem(
        "selectedFragrances",
        JSON.stringify([...newSelection])
      );
      return newSelection;
    });
  };

  const handleRecommendClick = () => {
    const storedFragrances = localStorage.getItem("selectedFragrances");
    const selectedFragrances = storedFragrances
      ? JSON.parse(storedFragrances)
      : [];
    if (selectedFragrances.length == 0) {
      setPopUpMessage({
        type: "warning",
        message: "Please select at least one fragrance before proceeding!",
      });
    } else {
      navigate("/recommend-fragrances");
    }
  };

  if (isLoading || status === "loading") {
    return <CircleLoader />;
  }
  return (
    <section className="bg-gray-50 min-h-screen py-10 mt-24">
      <div className="container mx-auto px-4 max-w-screen-xl">
        <h2 className="text-2xl font-bold mb-6 text-center text-brand-500">
          Recommend
        </h2>
        <div className="flex justify-center mb-6">
          <button
            onClick={handleRecommendClick}
            type="button"
            className="text-white bg-gradient-to-br from-green-400 to-blue-600 
                           hover:bg-gradient-to-bl focus:outline-none 
                           font-medium rounded-lg text-lg px-8 py-4 text-center"
          >
            Recommend
          </button>
        </div>
        <div className="grid gap-4 grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-4 w-full">
          {data?.pages?.map((page) =>
            page.fragrances.map((fragrance) => (
              <div
                key={fragrance.id}
                className="w-full max-w-xs mx-auto p-4 shadow-md"
              >
                <SelectableFragranceCard
                  {...fragrance}
                  id={fragrance.id}
                  selected={selectedFragrances.has(fragrance.id)}
                  onSelect={handleSelect}
                />
              </div>
            ))
          )}
        </div>
        <PopUpComponent />

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
