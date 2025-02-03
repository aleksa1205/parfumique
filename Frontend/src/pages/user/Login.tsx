import { useForm } from "react-hook-form";
import { DevTool } from "@hookform/devtools";
import { Link, Navigate, useLoaderData } from "react-router-dom";
import logo from "/src/assets/images/Parfumique logo.png";
import { UserLogin } from "../../dto-s/UserDto";
import useLoginMutation from "../../hooks/useLoginMutation";
import PasswordField from "../../components/form-fields/PasswordField";
import InputField from "../../components/form-fields/InputField";
import { CircleLoader } from "../../components/loaders/CircleLoader";
import UseAuth from "../../hooks/useAuth";
import usePopUpMessage from "../../hooks/Animated Components/usePopUpMessage";
import { useEffect } from "react";

export function loader({request}: any) {
  const url = new URL(request.url);
  return url.searchParams.get("message")
}

const Login = () => {

  const form = useForm<UserLogin>();
  const { register, control, handleSubmit, formState } = form;
  const { errors } = formState;
  const { loginMutation, credentialError } = useLoginMutation();
  const { auth } = UseAuth();

  const message = useLoaderData();
  const { PopUpComponent, setPopUpMessage } = usePopUpMessage();

  useEffect(() => {
    if(typeof message === 'string') {
      setPopUpMessage({
        message,
        type: 'info'
      })
    }
  }, [])

  if (auth.jwtToken !== '')
    return <Navigate to='/' replace />

  const onSubmit = async (userData: UserLogin) => {
    await loginMutation.mutateAsync(userData);
  };

  return (
    <>
    <PopUpComponent />
    <section className="bg-white font-roboto mt-14">
      {loginMutation.isLoading && <CircleLoader />}
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
              Login
            </h1>
            <form
              onSubmit={handleSubmit(onSubmit)}
              noValidate
              className="space-y-6"
            >
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
              {credentialError && (
                <div className="error text-center"> {credentialError}</div>
              )}

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
    </>
  );
};

export default Login;
