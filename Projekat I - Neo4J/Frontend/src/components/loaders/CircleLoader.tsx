import classes from "./CircleLoader.module.css";

export const CircleLoader = () => {
  return (
    <div className={classes.overlay}>
      <div className={classes.wrapper}>
        <div className={classes.circle}></div>
        <div className={classes.circle}></div>
        <div className={classes.circle}></div>
        <div className={classes.shadow}></div>
        <div className={classes.shadow}></div>
        <div className={classes.shadow}></div>
      </div>
    </div>
  );
};
