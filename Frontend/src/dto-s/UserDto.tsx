import { BaseFragrance } from "./FragranceDto";
import { Roles } from "../api/Roles";

export type UserLogin = {
  username: string;
  password: string;
  role: Roles;
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
  role: Roles;
};

export type GetUserResponse = {
  image: string;
  name: string;
  surname: string;
  gender: string;
  username: string;
  collection: Array<BaseFragrance>;
};

export type UpdateSelfDto = {
  name: string;
  surname: string;
  gender: string;
  image: string;
}