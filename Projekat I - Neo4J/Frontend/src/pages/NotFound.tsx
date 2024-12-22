import { Link } from "react-router-dom";

export const NotFound = () => {
  return (
    <section className="bg-white font-roboto">
      <div className="mx-auto max-w-screen-xl py-16 px-6">
        <div className="mx-auto  text-center">
          <h1 className="mb-4 font-extrabold text-9xl">404</h1>
          <p className="mb-4 text-3xl tracking-tight font-bold text-gray-900 md:text-4xl dark:text-white">
            Something's missing.
          </p>
          <p className="mb-4 text-lg my-text-gray">
            Sorry, we can't find that page. You'll find lots to explore on the
            home page.
          </p>
          <Link
            to="/"
            className="block py-2 mx-auto w-48 rounded-2xl my-active text-center"
          >
            Back to Homepage
          </Link>
        </div>
      </div>
    </section>
  );
};

export default NotFound;
