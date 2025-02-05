import { BaseFragrance } from "./FragranceDto";

export type Manufacturer = {
  name: string;
  image: string;
  fragrances: Array<BaseFragrance>;
};
