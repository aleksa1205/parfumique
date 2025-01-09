import { useForm } from "react-hook-form";
import { DevTool } from "@hookform/devtools";
import { Link, useNavigate } from "react-router-dom";
import logo from "../assets/images/logo.jpg";
import useUserController, { User } from "../api/controllers/useUserController";

const Register = () => {
  const form = useForm<User>();
  const { register, control, handleSubmit, formState } = form;
  const { errors } = formState;
  const navigate = useNavigate();
  const userController = useUserController();

  const onSubmit = async (data: User) => {
    try {
      await userController.registerUser(data);
      navigate("/login");
    } catch (error) {
      console.log(error);
    }
  };

  return (
    <section className="bg-white font-roboto">
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
              Register
            </h1>
            <form
              onSubmit={handleSubmit(onSubmit)}
              noValidate
              className="space-y-6"
            >
              <div className="pb-1">
                <label
                  htmlFor="name"
                  className="block mb-2 font-medium text-white"
                >
                  Name
                </label>
                <input
                  type="text"
                  id="name"
                  {...register("name", {
                    required: "Please fill in the name field to proceed!",
                  })}
                  className="bg-gray-50 border border-gray-300 text-gray-900 rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                  placeholder="Pera"
                  required
                />
                <p className="error">{errors.name?.message}</p>
              </div>
              <div className="pb-1">
                <label
                  htmlFor="surname"
                  className="block mb-2 font-medium text-white"
                >
                  Surname
                </label>
                <input
                  type="text"
                  id="surname"
                  {...register("surname", {
                    required: "Please fill in the surname field to proceed!",
                  })}
                  className="bg-gray-50 border border-gray-300 text-gray-900 rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                  placeholder="Perić"
                  required
                />
                <p className="error">{errors.surname?.message}</p>
              </div>
              <div>
                <label
                  htmlFor="gender"
                  className="block mb-2 font-medium text-white"
                >
                  Gender
                </label>
                <select
                  id="gender"
                  {...register("gender")}
                  className="bg-gray-50 border border-gray-300 text-gray-900 rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                >
                  <option value="M">M</option>
                  <option value="F">F</option>
                </select>
              </div>
              <div className="pb-1">
                <label
                  htmlFor="username"
                  className="block mb-2 text-sm font-medium text-white"
                >
                  Username
                </label>
                <input
                  type="text"
                  id="username"
                  {...register("username", {
                    required: "Please fill in the username field to proceed!",
                  })}
                  placeholder="peraperic"
                  className="bg-gray-50 border border-gray-300 text-gray-900 rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
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
              </div>

              <button
                type="submit"
                className="mt-5 w-full my-bg-brand focus:ring-4 focus:outline-none focus:ring-primary-300 font-medium rounded-lg px-5 py-2.5 text-center"
              >
                Register
              </button>
            </form>
            <DevTool control={control} />
          </div>
        </div>
      </div>
    </section>
  );
};

export default Register;
