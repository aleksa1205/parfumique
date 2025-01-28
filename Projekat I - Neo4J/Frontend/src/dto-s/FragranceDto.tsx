export type Fragrance = {
  id: number;
  image: string;
  name: string;
  batchYear: number;
  gender: string;
  manufacturer: {
    name: string;
  };
  perfumers: {
    id: number;
    name: string;
    surname: string;
    image: string;
  }[];
  top: {
    image: string;
    name: string;
  }[];
  middle: {
    image: string;
    name: string;
  }[];
  base: {
    image: string;
    name: string;
  }[];
};

export type BaseFragrance = Omit<
  Fragrance,
  "manufacturer" | "perfumers" | "top" | "middle" | "base" | "batchYear"
>;

export type FragrancePagination = {
  page: number;
  size: number;
  totalPages: number;
  fragrances: Array<BaseFragrance>;
};

export type FragranceInfinitePagination = {
  fragrances: BaseFragrance[];
  currentPage: number;
  nextPage: number | null;
};
