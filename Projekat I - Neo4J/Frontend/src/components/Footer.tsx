import { Link } from "react-router-dom";
import { FaGithub, FaLinkedin } from "react-icons/fa";
import logo from "../assets/images/logo.jpg";

const Footer = () => {
  return (
    <footer className="bg-white font-roboto">
      <div className="mx-auto w-full max-w-screen-xl p-4 py-6 lg:py-8">
        <div className="md:flex md:justify-between">
          <div className="mb-6 md:mb-0">
            <div className="flex items-center">
              <img
                src={logo}
                className="h-8 me-3 scale-150 mr-5"
                alt="Fragrance Recommendation"
              />
              <Link
                to="/"
                className="self-center text-xl font-semibold whitespace-nowrap my-text-black"
              >
                Fragrance Recommendation
              </Link>
            </div>
          </div>
          <div className="grid grid-cols-2 gap-8 sm:gap-6 sm:grid-cols-3">
            <div>
              <h2 className="mb-6 text-sm font-semibold uppercase text-black">
                Inspiration
              </h2>
              <ul className="font-medium">
                <li className="mb-4">
                  <a
                    href="https://www.fragrantica.com/"
                    target="_blank"
                    className="my-text-gray"
                  >
                    Fragrantica
                  </a>
                </li>
                <li>
                  <a
                    href="https://www.parfumo.com/"
                    target="_blank"
                    className="my-text-gray"
                  >
                    parfumo
                  </a>
                </li>
              </ul>
            </div>
            <div>
              <h2 className="mb-6 text-sm font-semibold uppercase text-black">
                Follow us
              </h2>
              <a
                href="https://github.com/aleksa1205/NapredneBazePodataka"
                target="_blank"
                className="my-text-gray font-medium"
              >
                Github
              </a>
            </div>
            <div>
              <h2 className="mb-6 text-sm font-semibold uppercase text-black">
                Pravo
              </h2>
              <ul className="font-medium">
                <li className="mb-4">
                  <a href="#" className="my-text-gray">
                    Privacy policy
                  </a>
                </li>
                <li>
                  <a href="#" className="my-text-gray">
                    Terms and Conditions
                  </a>
                </li>
              </ul>
            </div>
          </div>
        </div>
        <hr className="my-6 border-gray-200 sm:mx-auto lg:my-8" />
        <div className="sm:flex sm:items-center sm:justify-between">
          <span className="text-sm my-text-gray sm:text-center">
            © 2024 Aleksa Perić 18826 & Jovan Cvetković 18981. All rights
            retained.
          </span>
          <div className="flex mt-6 sm:justify-center sm:mt-0">
            <ul className="flex flex-row">
              <li className="mr-2">
                <a
                  href="https://www.linkedin.com/in/aleksaperic02/"
                  target="_blank"
                >
                  <FaLinkedin className="w-4 h-4 my-text-gray" />
                  <span className="sr-only">LinkedIn account Aleksa Perić</span>
                </a>
              </li>
              <li className="mr-2">
                <a
                  href="https://github.com/aleksa1205/NapredneBazePodataka"
                  target="_blank"
                >
                  <FaGithub className="w-4 h-4 my-text-gray" />
                  <span className="sr-only">GitHub account</span>
                </a>
              </li>

              <li>
                <a href="https://www.linkedin.com/in/cjovan02/" target="_blank">
                  <FaLinkedin className="w-4 h-4 my-text-gray" />
                  <span className="sr-only">
                    LinkedIn account Jovan Cvetković
                  </span>
                </a>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
