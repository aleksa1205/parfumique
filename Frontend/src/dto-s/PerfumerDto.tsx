import { BaseFragrance } from "./FragranceDto";

export type Perfumer = {
  name: string;
  surname: string;
  gender: string;
  country: string;
  image: string;
  fragrances: Array<BaseFragrance>;
};
