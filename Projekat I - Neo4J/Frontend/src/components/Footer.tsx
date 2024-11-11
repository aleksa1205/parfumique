import logo from "../assets/images/logo.jpg";
import { FaGithub } from "react-icons/fa";

const Footer = () => {
  return (
    <footer className="bg-black">
      <div className="mx-auto w-full max-w-screen-xl p-4 py-6 lg:py-8">
        <div className="md:flex md:justify-between">
          <div className="mb-6 md:mb-0">
            {/*Change to Link Tag*/}
            <div className="flex items-center">
              <img
                src={logo}
                className="h-8 me-3 scale-150 rounded-2xl mr-5"
                alt="Fragrance Recommendation"
              />
              <a
                href="#"
                className="self-center text-xl font-semibold whitespace-nowrap text-white hover:text-red-600"
              >
                Fragrance Recommendation
              </a>
            </div>
          </div>
          <div className="grid grid-cols-2 gap-8 sm:gap-6 sm:grid-cols-3">
            <div>
              <h2 className="mb-6 text-sm font-semibold uppercase text-white">
                Inspiracija
              </h2>
              <ul className="text-gray-400 font-medium">
                <li className="mb-4">
                  <a
                    href="https://www.fragrantica.com/"
                    target="_blank"
                    className="hover:text-red-600"
                  >
                    Fragrantica
                  </a>
                </li>
                <li>
                  <a
                    href="https://www.parfumo.com/"
                    target="_blank"
                    className="hover:text-red-600"
                  >
                    parfumo
                  </a>
                </li>
              </ul>
            </div>
            <div>
              <h2 className="mb-6 text-sm font-semibold uppercase text-white">
                Pratite nas
              </h2>
              <a
                href="https://github.com/aleksa1205/NapredneBazePodataka"
                target="_blank"
                className="hover:text-red-600 text-gray-400 font-medium"
              >
                Github
              </a>
            </div>
            <div>
              <h2 className="mb-6 text-sm font-semibold uppercase text-white">
                Pravo
              </h2>
              <ul className="text-gray-400 font-medium">
                <li className="mb-4">
                  <a href="#" className="hover:text-red-600">
                    Politika privatnosti
                  </a>
                </li>
                <li>
                  <a href="#" className="hover:text-red-600">
                    Uslovi korišćenja
                  </a>
                </li>
              </ul>
            </div>
          </div>
        </div>
        <hr className="my-6 border-gray-200 sm:mx-auto dark:border-gray-700 lg:my-8" />
        <div className="sm:flex sm:items-center sm:justify-between">
          <span className="text-sm text-gray-500 sm:text-center dark:text-gray-400">
            © 2024 Aleksa Perić 18826 & Jovan Cvetković. Sva prava zadržana.
          </span>
          <div className="flex mt-6 sm:justify-center sm:mt-0">
            <a
              href="https://github.com/aleksa1205/NapredneBazePodataka"
              target="_blank"
              className="text-gray-500 hover:text-white ms-5"
            >
              <FaGithub className="w-4 h-4" />
              <span className="sr-only">GitHub account</span>
            </a>
          </div>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
