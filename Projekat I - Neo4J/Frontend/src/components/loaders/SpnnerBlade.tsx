import classes from './SpinnerBlade.module.css'

export function SpinnerBlade() {
    return (
    <div className={`${classes.spinner} classes.center`}>
        <div className={classes.spinnerBlade}></div>
        <div className={classes.spinnerBlade}></div>
        <div className={classes.spinnerBlade}></div>
        <div className={classes.spinnerBlade}></div>
        <div className={classes.spinnerBlade}></div>
        <div className={classes.spinnerBlade}></div>
        <div className={classes.spinnerBlade}></div>
        <div className={classes.spinnerBlade}></div>
        <div className={classes.spinnerBlade}></div>
        <div className={classes.spinnerBlade}></div>
        <div className={classes.spinnerBlade}></div>
        <div className={classes.spinnerBlade}></div>
    </div>
    )
}