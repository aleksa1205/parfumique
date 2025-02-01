import { Link, useLocation } from "react-router-dom";
import logo from "../assets/images/Parfumique logo.png";
import useIsLoggedIn from "../hooks/useIsLoggedIn";
import { useContext, useEffect, useRef, useState } from "react";
import { CurrUserContext } from "../context/CurrUserProvider";
import { CircleLoader } from "./loaders/CircleLoader";
import useLogout from "../hooks/useLogout";
import NavLinks from "./NavLinks";
import UserImage from "./UserImage";
const Navbar = () => {
  const isLoggedIn = useIsLoggedIn();
  const logout = useLogout();
  const location = useLocation();
  const { user, isLoading } = useContext(CurrUserContext);
  const [dropdownOpen, setDropdownOpen] = useState(false);
  const [isScrolled, setIsScrolled] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

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

  useEffect(() => {
    setDropdownOpen(false);
  }, [location]);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (
        dropdownRef.current &&
        !dropdownRef.current.contains(event.target as Node)
      ) {
        setDropdownOpen(false);
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const toggleDropdown = () => {
    setDropdownOpen((prevState) => !prevState);
  };

  if (isLoggedIn && isLoading) {
    return <CircleLoader />;
  }

  return (
    <header
      className={`fixed z-50 top-0 bg-white w-full border-b transition-all ${
        isScrolled ? "border-neutral-400" : "border-white"
      }`}
    >
      <nav className="border-gray-200 font-roboto max-w-screen-xl max-w-screen-xl mx-auto">
        <div className="flex flex-wrap items-center justify-between p-4">
          <Link
            to="/"
            className="flex items-center space-x-3 rtl:space-x-reverse"
          >
            <img src={logo} className="h-11 scale-150 rounded-2xl " alt="logo" />
          </Link>
          <NavLinks />
          <div className="w-full md:block md:w-auto">
            {isLoggedIn ? (
              <div className="relative">
                <button
                  onClick={toggleDropdown}
                  className="flex items-center justify-center w-10 h-10 rounded-full bg-gray-200"
                >
                  <UserImage />
                </button>
                {dropdownOpen && (
                  <div
                    ref={dropdownRef}
                    className="absolute mt-2 z-50 right-0 w-48 shadow-lg my-4 text-base list-none my-bg-light divide-y divide-gray-100 rounded-lg"
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
                        <Link
                          to="/user-profile"
                          className="block px-4 py-2 text-sm my-text-black"
                        >
                          Profile details
                        </Link>
                      </li>
                      <li>
                        <Link
                          to="/user-fragrances"
                          className="block px-4 py-2 text-sm my-text-black"
                        >
                          Fragrances
                        </Link>
                      </li>
                      <li>
                        <a
                          href="#"
                          className="block px-4 py-2 text-sm my-text-black"
                          onClick={() => logout()}
                        >
                          Log out
                        </a>
                      </li>
                    </ul>
                  </div>
                )}
              </div>
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
