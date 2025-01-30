import { ReactNode } from "react";
import { FaCircleInfo } from "react-icons/fa6";

type PropsValues = {
  children: ReactNode;
};

const WarningPopUp = ({ children }: PropsValues) => {
  return (
    <div
      className="flex items-center p-4 mb-4 text-sm text-yellow-800 border border-yellow-300 rounded-lg bg-yellow-50"
      role="alert"
    >
      <FaCircleInfo />
      <div className="ml-3">{children}</div>
    </div>
  );
};

export default WarningPopUp;
