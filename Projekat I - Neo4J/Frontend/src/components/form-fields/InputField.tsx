import { InputFieldProps } from "../../dto-s/Props";

const InputField = ({
  register,
  error,
  id,
  label,
  placeholder = "",
  type = "text",
  validationRules,
}: InputFieldProps) => {
  return (
    <div className="pb-1">
      <label htmlFor={id} className="block mb-2 font-medium text-white">
        {label}
      </label>
      <input
        id={id}
        placeholder={placeholder}
        type={type}
        {...register(id, validationRules)}
        required
        className="bg-gray-50 border border-gray-300 text-gray-900 rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
      />
      <p className="error">{error?.message}</p>
    </div>
  );
};

export default InputField;
