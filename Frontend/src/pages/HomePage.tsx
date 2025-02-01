import FirstBlock from "../components/home/FirstBlock";
import SecondBlock from "../components/home/SecondBlock";
import AboutUsBlock from "../components/home/AboutUsBlock";
import useLogout from "../hooks/useLogout";

const HomePage = () => {
  const logout = useLogout();

  return (
    <div>
      <FirstBlock />
      <button onClick={() => logout()}>No Mess</button>
      <button onClick={() => logout("Session expired, please log in again.")}>Yes Mess</button>
      <SecondBlock />
      <AboutUsBlock />
    </div>
  );
};

export default HomePage;
