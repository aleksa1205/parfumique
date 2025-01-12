export type PaginationProps = {
  page: number;
  setPage: (page: number) => void;
  numberOfPages: number;
  isPreviousData: boolean;
};

export type FragranceCardProps = {
  id: string;
  image: string;
  name: string;
  gender: string;
};
