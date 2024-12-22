import axios from "axios";
const BASE_URL = "https://localhost:8080";

export const client = axios.create({
  baseURL: BASE_URL,
});
