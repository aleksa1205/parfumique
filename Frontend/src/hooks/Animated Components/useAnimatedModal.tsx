import { useEffect, useState } from "react";
import classes from "./ModalAnimated.module.css";
import { animated } from "@react-spring/web";
import useModalAnimation from "../Animations/useModalAnimation";

export default function useAnimatedModal() {

  const [isSmallScreen, setIsSmallScreen] = useState(window.innerWidth < 700);
    const { closeModal, openModal, transition } = useModalAnimation()

  useEffect(() => {
    const handleResize = () => {
      setIsSmallScreen(window.innerWidth < 700);
    };

    window.addEventListener("resize", handleResize);

    return () => window.removeEventListener("resize", handleResize);
  }, []);

  useEffect(() => {
    document.body.style.overflow = "hidden";

    return () => {
      document.body.style.overflow = "auto";
    };
  }, []);

  const AnimatedModal = ({children}: {children: any}) => {
    return (
    <>
        {transition((style, showModal) => {
            return showModal ? (
                <>
                    <animated.div
                        style={{ opacity: style.t.to((value: number) => value)}}
                        onClick={() => closeModal()}
                        className={classes.backdrop}
                    />

                    <animated.dialog
                        style={{
                        opacity: style.t.to((value) => value),
                        top: isSmallScreen
                            ? ''
                            : style.bot.to((value: string) => value),
                        bottom: isSmallScreen
                                ? style.botSmall.to((value: string) => value)
                                : '',
                        transform: isSmallScreen 
                                    ? 'translate(-50%, 0)'
                                    : style.scale.to((value: number) => `translate(-50%, -50%) scale(${value})`),
                        }}
                        open
                        className={classes.modal}
                    >
                        {children}
                    </animated.dialog>
                </>
            ) : null
        })}

    </>
  )};

  return { AnimatedModal, openModal, closeModal }
}
