import { SelectFieldProps } from "../../dto-s/Props";

const SelectField = ({ register, id, label, options }: SelectFieldProps) => {
  return (
    <div>
      <label htmlFor={id} className="block mb-2 font-medium text-white">
        {label}
      </label>
      <select
        id={id}
        {...register(id)}
        className="bg-gray-50 border border-gray-300 text-gray-900 rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
      >
        {options.map((item) => (
          <option key={item} value={item}>
            {item}
          </option>
        ))}
      </select>
    </div>
  );
};

export default SelectField;
