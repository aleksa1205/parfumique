import { NavLink } from "react-router-dom";

const navigation = [
  { name: "Homepage", link: "/" },
  { name: "Fragrances", link: "/fragrances" },
  { name: "About us", link: "/about-us" },
];

const NavLinks = () => {
  return (
    <div className="hidden w-full md:block md:w-auto " id="navbar-default">
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
  );
};

export default NavLinks;
