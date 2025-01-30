import classes from "./Loader.module.css";

export const Loader = () => {
  return (
    <div className={classes.loader}>
      <span className={classes.loaderText}>Loading</span>
      <span className={classes.load}></span>
    </div>
  );
};
