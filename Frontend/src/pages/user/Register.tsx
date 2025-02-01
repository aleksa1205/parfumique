import { useForm } from "react-hook-form";
import { DevTool } from "@hookform/devtools";
import { Link } from "react-router-dom";
import { User } from "../../dto-s/UserDto";
import logo from "/src/assets/images/Parfumique logo.png";
import PasswordField from "../../components/form-fields/PasswordField";
import InputField from "../../components/form-fields/InputField";
import SelectField from "../../components/form-fields/SelectField";
import useUserRegisterMutation from "../../hooks/useUserRegisterMutation";
import { CircleLoader } from "../../components/loaders/CircleLoader";

const Register = () => {
  const form = useForm<User>();
  const { registerMutation, registerError } = useUserRegisterMutation();
  const { register, control, handleSubmit, formState } = form;
  const { errors } = formState;

  const onSubmit = async (user: User) => {
    await registerMutation.mutateAsync(user);
  };

  return (
    <section className="bg-white font-roboto mt-14">
      {registerMutation.isLoading && <CircleLoader />}
      <div className="flex flex-col items-center justify-center px-6 py-8 mx-auto">
        <Link
          to="/"
          className="flex items-center mb-6 text-2xl font-semibold my-text-black"
        >
          <img
            className="h-12 mr-5 rounded-2xl scale-150"
            src={logo}
            alt="logo"
          />
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
              <InputField
                register={register}
                error={errors.name}
                id="name"
                label="Name"
                placeholder="Pera"
                validationRules={{
                  required: "Please fill in the name field to proceed!",
                  minLength: {
                    value: 3,
                    message: "Name must be at least 3 characters long",
                  },
                }}
              />
              <InputField
                register={register}
                error={errors.surname}
                id="surname"
                label="Surname"
                placeholder="Perić"
                validationRules={{
                  required: "Please fill in the surname field to proceed!",
                  minLength: {
                    value: 3,
                    message: "Surname must be at least 3 characters long",
                  },
                }}
              />
              <SelectField
                register={register}
                id="gender"
                label="Gender"
                options={["M", "F"]}
              />
              <InputField
                register={register}
                error={errors.username}
                id="username"
                label="Username"
                placeholder="peraperic"
                validationRules={{
                  required: "Please fill in the username field to proceed!",
                  minLength: {
                    value: 3,
                    message: "Username must be at least 3 characters long",
                  },
                }}
              />
              <PasswordField register={register} error={errors.password} />
              {registerError && (
                <div className="error text-center">{registerError}</div>
              )}
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
