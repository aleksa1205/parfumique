import { Link } from "react-router-dom";

const SecondBlock = () => {
  return (
    <div className="flex items-start justify-center bg-gradient-to-r my-text-black text-center p-6 font-roboto pt-12">
      <div className="flex md:flex-row flex-col items-center justify-center text-center w-full max-w-7xl p-6">
        <div className="ml-auto pl-10 w-full md:w-2/3 text-right">
          <p className="ml-auto text-lg mb-6 max-w-lg w-2/3">
            Discover and curate your personal fragrance collection with ease.
            Add your favorite perfumes to your profile, explore new scents, and
            browse our comprehensive fragrance database. Whether you're a
            collector or a curious enthusiast, our platform makes it simple to
            organize and explore the world of fragrances.
          </p>
          <Link
            to="/fragrances"
            className="inline-block px-6 py-2.5 text-xl font-semibold bg-white my-text-primary rounded-lg shadow-md hover:bg-gray-200 transition duration-300"
          >
            Fragrances
          </Link>
        </div>
        <div className="flex-shrink-0 w-full md:w-1/3">
          <img
            src="/src/assets/images/frag-second-image.png"
            alt="fragrance image home page"
            className="w-full h-auto object-cover rounded-lg"
          />
        </div>
      </div>
    </div>
  );
};

export default SecondBlock;
