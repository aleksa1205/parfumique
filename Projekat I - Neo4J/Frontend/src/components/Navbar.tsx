import { Link } from "react-router-dom";
import logo from "../assets/images/logo.jpg";

const Navbar = () => {
  return (
    <header>
      <nav className="border-gray-200 bg-gray-900">
        <div className="max-w-screen-xl flex flex-wrap items-center justify-between mx-auto p-4">
          <Link
            to="/"
            className="flex items-center space-x-3 rtl:space-x-reverse"
          >
            <img src={logo} className="h-8 scale-150 rounded-2xl" alt="logo" />
          </Link>
          <div className="hidden w-full md:block md:w-auto" id="navbar-default">
            <ul className="font-medium flex flex-col p-4 md:p-0 mt-4 border border-gray-100 rounded-lg  md:flex-row md:space-x-8 rtl:space-x-reverse md:mt-0 md:border-0 md:bg-white dark:bg-gray-800 md:dark:bg-gray-900 dark:border-gray-700">
              <li>
                <Link
                  to="/"
                  className="block py-2 px-3 text-white rounded md:bg-transparent hover:text-red-600 md:text-blue-700 md:p-0 dark:text-white md:dark:text-blue-500"
                  aria-current="page"
                >
                  Početna
                </Link>
              </li>
              {/*Change to Link Tag*/}
              <li>
                <a
                  href="#"
                  className="block py-2 px-3 text-white rounded hover:text-red-600 md:border-0 md:p-0 dark:hover:bg-gray-700 md:dark:hover:bg-transparent"
                >
                  Parfemi
                </a>
              </li>
              {/*Change to Link Tag*/}
              <li>
                <a
                  href="#"
                  className="block py-2 px-3 text-white rounded hover:text-red-600 md:border-0 md:p-0 dark:hover:bg-gray-700 md:dark:hover:bg-transparent"
                >
                  O nama
                </a>
              </li>
            </ul>
          </div>
          <div className="hidden w-full md:block md:w-auto">
            <Link
              to="/login"
              className="block py-2 px-3 bg-blue-700 rounded-2xl text-white hover:bg-blue-500 hover:text-black"
            >
              Prijavi se
            </Link>
          </div>
        </div>
      </nav>
    </header>
  );
};

export default Navbar;
