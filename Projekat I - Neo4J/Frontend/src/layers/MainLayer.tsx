import { Outlet } from "react-router-dom";
import Navbar from "../components/Navbar";
import Footer from "../components/Footer";

const MainLayer = () => {
  return (
    <span className="font-roboto">
      <Navbar />
      <Outlet />
      <Footer />
    </span>
  );
};

export default MainLayer;
