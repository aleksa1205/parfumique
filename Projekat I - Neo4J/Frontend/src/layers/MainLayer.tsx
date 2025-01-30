import { Outlet } from "react-router-dom";
import Navbar from "../components/Navbar";
import Footer from "../components/Footer";
import { CurrUserProvider } from "../context/CurrUserProvider";

const MainLayer = () => {
  return (
    <>
      <CurrUserProvider>
        <Navbar />
        <Outlet />
        <Footer />
      </CurrUserProvider>
    </>
  );
};

export default MainLayer;
