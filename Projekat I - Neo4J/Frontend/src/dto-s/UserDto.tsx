import { BaseFragrance } from "./FragranceDto";

export type UserLogin = {
  username: string;
  password: string;
};

export type User = {
  name: string;
  surname: string;
  gender: string;
  username: string;
  password: string;
};

export type LoginResponse = {
  username: string;
  token: string;
};

export type GetUserResponse = {
  image: string;
  name: string;
  surname: string;
  gender: string;
  username: string;
  collection: Array<BaseFragrance>;
};
