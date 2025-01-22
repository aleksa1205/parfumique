import { Link, NavLink } from "react-router-dom";
import logo from "../assets/images/logo.jpg";
import useIsLoggedIn from "../hooks/useIsLoggedIn";
import { useContext, useEffect, useState } from "react";
import { CurrUserContext } from "../context/CurrUserProvider";
import { CircleLoader } from "./loaders/CircleLoader";
import useLogout from "../hooks/useLogout";
import UseAuth from "../hooks/useAuth";
import { Roles } from "../api/Roles";

const navigation = [
  { name: "Homepage", link: "/" },
  { name: "Fragrances", link: "/fragrances" },
  { name: "About us", link: "/about-us" }
];

const Navbar = () => {
  const isLoggedIn = useIsLoggedIn();
  const logout = useLogout();
  const { user, isLoading } = useContext(CurrUserContext);
  const [dropdownOpen, setDropdownOpen] = useState(false);
  const { auth } = UseAuth();
  const [isScrolled, setIsScrolled] = useState(false);

  useEffect(() => {
    const handleScroll = () => {
      setIsScrolled(window.scrollY > 50);
    };

    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  }, []);

  useEffect(() => {
    if (!isLoggedIn) {
      setDropdownOpen(false);
    }
  }, [isLoggedIn]);

  const toggleDropdown = () => {
    setDropdownOpen((prevState) => !prevState);
  };

  if (isLoggedIn && isLoading) {
    return (
      <section className="bg-gray-50 antialiased py-12 h-screen flex justify-center items-center">
        <CircleLoader />
      </section>
    );
  }

  return (
    <header className={`fixed z-50 top-0 bg-white w-full border-b transition-all ${isScrolled ? 'border-neutral-400' : 'border-white'}`}>
      <nav className="border-gray-200 font-roboto max-w-screen-xl max-w-screen-xl mx-auto">
        <div className="flex flex-wrap items-center justify-between p-4">
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
            {auth.role == Roles.Admin && (
                <li>
                  <NavLink
                  to="/admin-dashboard"
                  className={({ isActive }) => {
                    return (
                      "rounded-md block py-2 px-5 " +
                      (isActive ? "my-active" : "my-text-gray")
                    );
                  }}
                  >
                    Admin Dashboard
                  </NavLink>
                </li>
              )}
              
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
          <div className="w-full md:block md:w-auto">
            {isLoggedIn ? (
              <>
                <button
                  onClick={toggleDropdown}
                  className="flex items-center justify-center w-10 h-10 rounded-full bg-gray-200"
                >
                  <img
                    src={
                      user?.image != ""
                        ? user?.image
                        : "https://via.placeholder.com/40"
                    }
                    alt="Profile"
                    className="rounded-full"
                  />
                </button>
                {dropdownOpen && (
                  <div
                    className="absolute mt-2 z-50 my-4 text-base list-none my-bg-light divide-y divide-gray-100 rounded-lg shadow"
                    id="user-dropdown"
                  >
                    <div className="px-4 py-3">
                      <span className="block text-sm my-text-black">
                        {user?.name} {} {user?.surname}
                      </span>
                      <span className="block text-sm my-text-gray truncate">
                        {user?.username}
                      </span>
                    </div>
                    <ul className="py-2" aria-labelledby="user-menu-button">
                      <li>
                        <a
                          href="#"
                          className="block px-4 py-2 text-sm my-text-black"
                        >
                          Profile details
                        </a>
                      </li>
                      <li>
                        <a
                          href="#"
                          className="block px-4 py-2 text-sm my-text-black"
                        >
                          Fragrances
                        </a>
                      </li>
                      <li>
                        <a
                          href="#"
                          className="block px-4 py-2 text-sm my-text-black"
                          onClick={logout}
                        >
                          Log out
                        </a>
                      </li>
                    </ul>
                  </div>
                )}
              </>
            ) : (
              <Link
                to="/login"
                className="block py-2 px-3 rounded-2xl my-active"
              >
                Log in
              </Link>
            )}
          </div>
        </div>
      </nav>
    </header>
  );
};

export default Navbar;
