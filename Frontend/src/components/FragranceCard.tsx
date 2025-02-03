import { Link, useSearchParams } from "react-router-dom";
import { base64ToUrl } from "../utils";
import { BaseFragrance } from "../dto-s/FragranceDto";

const FragranceCard = ({ id, name, image, gender }: BaseFragrance) => {
  const [searchParams] = useSearchParams();
  const currentPage = searchParams.get("page") || "0";
  return (
    <div className="relative grid gap-4 w-full">
      <div className="rounded-lg border border-gray-200 bg-white p-6 shadow-sm">
        <div className="h-56">
          <a href="#">
            <img
              className="mx-auto h-full rounded-xl object-cover"
              src={base64ToUrl(image)}
              alt={`${name} image`}
            />
          </a>
        </div>

        <div className="flex items-center h-12 justify-center">
          <Link
            to={`/fragrances/${id}?page=${currentPage}`}
            className="text-lg text-center font-semibold leading-tight my-text-black"
          >
            {`${name} for ${gender}`}
          </Link>
        </div>
      </div>
    </div>
  );
};

export default FragranceCard;
