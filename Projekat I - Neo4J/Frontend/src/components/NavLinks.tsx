import { NavLink } from "react-router-dom";
import UseAuth from "../hooks/useAuth";
import { Roles } from "../api/Roles";

const navigation = [
  { name: "Homepage", link: "/" },
  { name: "Fragrances", link: "/fragrances" },
  { name: "About us", link: "/about-us" },
];

const NavLinks = () => {
  const { auth } = UseAuth();
  return (
    <div className="hidden w-full md:block md:w-auto " id="navbar-default">
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
  );
};

export default NavLinks;
