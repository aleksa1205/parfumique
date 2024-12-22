import { Link, NavLink } from "react-router-dom";
import logo from "../assets/images/logo.jpg";

const navigation = [
  { name: "Homepage", link: "/" },
  { name: "Fragrances", link: "/fragrances" },
  { name: "About us", link: "/about-us" },
];

const Navbar = () => {
  return (
    <header>
      <nav className="border-gray-200 bg-white font-roboto">
        <div className="max-w-screen-xl flex flex-wrap items-center justify-between mx-auto p-4">
          <Link
            to="/"
            className="flex items-center space-x-3 rtl:space-x-reverse"
          >
            <img src={logo} className="h-8 scale-150 rounded-2xl " alt="logo" />
          </Link>
          <div
            className="hidden w-full md:block md:w-auto "
            id="navbar-default"
          >
            <ul className="font-medium flex flex-col p-4 md:p-0 mt-4 border border-gray-100 rounded-lg  md:flex-row md:space-x-8 rtl:space-x-reverse md:mt-0 md:border-0">
              {navigation.map((item) => (
                <li key={item.name}>
                  <NavLink
                    to={item.link}
                    className={({ isActive }) => {
                      return (
                        "rounded-md block py-2 px-5 " +
                        (isActive ? "my-active" : "my-text-gray")
                      );
                    }}
                  >
                    {item.name}
                  </NavLink>
                </li>
              ))}
            </ul>
          </div>
          <div className="hidden w-full md:block md:w-auto">
            <Link to="/login" className="block py-2 px-3 rounded-2xl my-active">
              Log in
            </Link>
          </div>
        </div>
      </nav>
    </header>
  );
};

export default Navbar;
