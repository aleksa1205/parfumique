import { Link } from "react-router-dom";
import useIsLoggedIn from "../../hooks/useIsLoggedIn";

const FirstBlock = () => {
  const isLoggedIn = useIsLoggedIn();
  return (
    <div className="flex items-start justify-center bg-gradient-to-r my-text-black text-center p-6 font-roboto pt-12 mt-16">
      <div className="flex md:flex-row flex-col items-center justify-center text-center w-full max-w-7xl p-6">
        <div className="flex-shrink-0 w-full md:w-1/3">
          <img
            src="/src/assets/images/frag-first-image.jpg"
            alt="fragrance image home page"
            className="w-full h-auto object-cover rounded-lg"
          />
        </div>
        <div className="pl-10 w-full md:w-2/3 text-left">
          <h1 className="text-3xl font-bold leading-tight mb-4">
            Unlock the World of Fragrances
          </h1>
          <p className="text-lg mb-6 max-w-lg w-2/3">
            Discover a personalized fragrance journey with our platform. Get
            recommendations tailored to your preferences and explore new scents.
          </p>
          {!isLoggedIn && (
            <Link
              to="/register"
              className="inline-block px-6 py-2.5 text-xl font-semibold bg-white my-text-primary rounded-lg shadow-md hover:bg-gray-200 transition duration-300"
            >
              Join Us
            </Link>
          )}
        </div>
      </div>
    </div>
  );
};

export default FirstBlock;
