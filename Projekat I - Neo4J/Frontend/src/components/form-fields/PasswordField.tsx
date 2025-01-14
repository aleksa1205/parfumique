import { useState } from "react";
import { FaEye, FaEyeSlash } from "react-icons/fa";
import { PasswordFieldProps } from "../../dto-s/Props";

const PasswordField: React.FC<PasswordFieldProps> = ({ register, error }) => {
  const [showPassword, setShowPassword] = useState(false);

  const togglePasswordVisibility = () => {
    setShowPassword((prev) => !prev);
  };

  return (
    <div className="pb-1">
      <label
        htmlFor="password"
        className="block mb-2 text-sm font-medium text-white"
      >
        Password
      </label>
      <div className="relative flex items-center">
        <input
          type={showPassword ? "text" : "password"}
          id="password"
          {...register("password", {
            required: "Please fill in the password field to proceed!",
            minLength: {
              value: 8,
              message: "Password must be at least 8 characters long!",
            },
            pattern: {
              value: /^(?=.*\d).+$/,
              message: "Password must contain at least one number!",
            },
          })}
          placeholder="••••••••"
          className="bg-gray-50 border border-gray-300 text-gray-900 rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
          required
        />
        <button
          type="button"
          className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 w-8 flex items-center justify-center"
          onClick={togglePasswordVisibility}
        >
          {showPassword ? <FaEyeSlash /> : <FaEye />}
        </button>
      </div>
      <p className="error">{error?.message}</p>
    </div>
  );
};

export default PasswordField;
