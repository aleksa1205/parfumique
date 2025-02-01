import React from "react";
import FragranceCard from "./FragranceCard";
import { SelectableFragranceCardProps } from "../dto-s/Props";

const SelectableFragranceCard: React.FC<SelectableFragranceCardProps> = ({
  id,
  image,
  name,
  gender,
  onSelect,
  selected,
}) => {
  return (
    <div
      className={`rounded-lg border p-4 shadow-sm cursor-pointer transition-all duration-200 
      ${selected ? "border-brand-500 bg-brand-100" : "border-gray-200 bg-white"}
    `}
      onClick={() => onSelect(id, !selected)}
    >
      <div className="relative">
        <FragranceCard id={id} image={image} name={name} gender={gender} />
      </div>
    </div>
  );
};

export default SelectableFragranceCard;
