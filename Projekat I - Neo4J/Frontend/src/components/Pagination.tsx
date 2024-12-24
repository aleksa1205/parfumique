interface PaginationProps {
  page: number;
  setPage: (page: number) => void;
  numberOfPages: number;
  isPreviousData: boolean;
}

const Pagination: React.FC<PaginationProps> = ({
  page,
  setPage,
  numberOfPages,
  isPreviousData,
}) => {
  const getDisplayPages = () => {
    if (numberOfPages <= 3) {
      return Array.from({ length: numberOfPages }, (_, i) => i + 1);
    } else if (page === 1 && numberOfPages > 3) {
      return [1, 2, 3];
    } else if (page === numberOfPages) {
      return [numberOfPages - 1, numberOfPages];
    }
    return [page - 1, page, page + 1];
  };

  const pagesArray = getDisplayPages();
  return (
    <div className="flex justify-center mt-4 font-roboto">
      <nav aria-label="Page navigation">
        <ul className="flex items-center -space-x-px h-10 text-base">
          {/* Previous button */}
          <li>
            <button
              onClick={() => setPage(page - 1)}
              disabled={page === 1 || isPreviousData}
              className={`items-center justify-center px-4 h-10 ms-0 leading-tight bg-white border border-gray-300 rounded-s-lg ${
                page === 1
                  ? "my-text-gray cursor-not-allowed"
                  : "my-text-primary"
              }`}
            >
              <span className="sr-only">Previous</span>
              <svg
                className="w-3 h-3 rtl:rotate-180"
                aria-hidden="true"
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 6 10"
              >
                <path
                  stroke="currentColor"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M5 1 1 5l4 4"
                />
              </svg>
            </button>
          </li>
          {/* First page and ... */}
          {page > 2 && (
            <>
              <li>
                <button
                  onClick={() => setPage(1)}
                  disabled={page === 1}
                  className="items-center justify-center px-4 h-10 ms-0 leading-tight bg-white border border-gray-300 my-text-primary"
                >
                  1
                </button>
              </li>
              {page > 3 && (
                <li className="flex items-center justify-center px-2 h-10 ms-0 leading-tight bg-white border border-gray-300 my-text-primary">
                  ...
                </li>
              )}
            </>
          )}

          {/* Dynamic page numbers */}
          {pagesArray.map((pg) => (
            <li key={pg}>
              <button
                onClick={() => setPage(pg)}
                disabled={isPreviousData}
                className="items-center justify-center px-4 h-10 ms-0 leading-tight bg-white border border-gray-300 my-text-primary"
              >
                {pg}
              </button>
            </li>
          ))}

          {/* Last page and ... */}
          {page < numberOfPages - 1 && (
            <>
              {page < numberOfPages - 2 && (
                <li className="flex items-center justify-center px-2 h-10 ms-0 leading-tight bg-white border border-gray-300 my-text-primary">
                  ...
                </li>
              )}
              <li>
                <button
                  onClick={() => setPage(page - 1)}
                  disabled={page === numberOfPages}
                  className="items-center justify-center px-4 h-10 ms-0 leading-tight bg-white border border-gray-300 my-text-primary"
                >
                  {numberOfPages}
                </button>
              </li>
            </>
          )}
          {/* Next button */}
          <li>
            <button
              onClick={() => setPage(page + 1)}
              disabled={page === numberOfPages || isPreviousData}
              className={`items-center justify-center px-4 h-10 ms-0 leading-tight bg-white border border-gray-300 rounded-e-lg ${
                page === numberOfPages
                  ? "my-text-gray cursor-not-allowed"
                  : "my-text-primary"
              }`}
            >
              <span className="sr-only">Next</span>
              <svg
                className="w-3 h-3 rtl:rotate-180"
                aria-hidden="true"
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 6 10"
              >
                <path
                  stroke="currentColor"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="m1 9 4-4-4-4"
                />
              </svg>
            </button>
          </li>
        </ul>
      </nav>
    </div>
  );
};

export default Pagination;
