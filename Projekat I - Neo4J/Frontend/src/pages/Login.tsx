import { useForm } from "react-hook-form";
import { DevTool } from "@hookform/devtools";
import { Link, useNavigate } from "react-router-dom";
import logo from "../assets/images/logo.jpg";
import useUserController from "../api/controllers/useUserController";
import { useMutation } from "@tanstack/react-query";
import { useState } from "react";
import UseAuth from "../hooks/useAuth";
import { UserLogin } from "../dto-s/UserDto";
import { WrongCredentials } from "../dto-s/Errors";

const Login = () => {
  const form = useForm<UserLogin>();
  const { register, control, handleSubmit, formState } = form;
  const { login } = useUserController();
  const { errors } = formState;
  const [credentialError, setCredentialError] = useState<string | null>(null);
  const { setAuth } = UseAuth();
  const navigate = useNavigate();

  const loginMutation = useMutation((user: UserLogin) => login(user), {
    onSuccess: (response) => {
      setAuth({ jwtToken: response.token, username: response.username, role: response.role });
      setCredentialError(null);
      //change to user profile
      navigate("/");
    },
    onError: (error) => {
      if (error instanceof WrongCredentials) {
        setCredentialError("Invalid username or password!");
      } else {
        setCredentialError("Error 500");
      }
    },
  });

  const onSubmit = async (userData: UserLogin) => {
    loginMutation.mutateAsync(userData);
  };

  return (
    <section className="bg-white font-roboto mt-16">
      <div className="flex flex-col items-center justify-center px-6 py-8 mx-auto">
        <Link
          to="/"
          className="flex items-center mb-6 text-2xl font-semibold my-text-black"
        >
          <img
            className="w-8 h-8 mr-5 rounded-2xl scale-150"
            src={logo}
            alt="logo"
          />
          Fragrance Recommendation
        </Link>
        <div className="w-full bg-black text-white rounded-lg shadow border md:mt-0 sm:max-w-md xl:p-0">
          <div className="p-6 space-y-4 md:space-y-6 sm:p-8">
            <h1 className="text-xl font-bold text-center leading-tight tracking-tight md:text-2xl">
              Login
            </h1>
            <form
              onSubmit={handleSubmit(onSubmit)}
              noValidate
              className="space-y-6"
            >
              <div className="pb-1">
                <label
                  htmlFor="username"
                  className="block mb-2 font-medium text-white"
                >
                  Username
                </label>
                <input
                  type="text"
                  id="username"
                  {...register("username", {
                    required: "Please fill in the username field to proceed!",
                  })}
                  className="bg-gray-50 border border-gray-300 text-gray-900 rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                  placeholder="peraperic"
                  required
                />
                <p className="error">{errors.username?.message}</p>
              </div>
              <div className="pb-1">
                <label
                  htmlFor="password"
                  className="block mb-2 text-sm font-medium text-white"
                >
                  Password
                </label>
                <input
                  type="text"
                  id="password"
                  {...register("password", {
                    required: "Please fill in the password field to proceed!",
                    minLength: {
                      value: 8,
                      message: "Password must be at least 8 charachters long!",
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
                <p className="error">{errors.password?.message}</p>
                {credentialError && (
                  <div className="error text-sm font-medium pt-5">
                    {credentialError}
                  </div>
                )}
              </div>

              <button
                type="submit"
                className="mt-5 w-full my-bg-brand focus:ring-4 focus:outline-none focus:ring-primary-300 font-medium rounded-lg px-5 py-2.5 text-center"
              >
                Log in
              </button>
              <p className="text-sm font-ligh my-text-gray">
                Don't have an account?{" "}
                <Link to="/register" className="font-medium my-text-gray">
                  Register
                </Link>
              </p>
            </form>
            <DevTool control={control} />
          </div>
        </div>
      </div>
    </section>
  );
};

export default Login;
