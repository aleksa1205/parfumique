import { FaGithub, FaLinkedin } from "react-icons/fa";

const AboutUs = () => {
  return (
    <section className="bg-gray-50 antialiased py-12">
      <div className="mx-auto max-w-screen-xl px-4 text-center">
        <h1 className="text-4xl font-bold mb-8">
          About <span className="my-text-medium">Us</span>
        </h1>
        <p className="text-lg mb-8">
          We're a team of students dedicated to learning and growing as
          developers. We're constantly working hard to improve our skills and
          gain practical experience in building software. Our focus is on
          learning as much as we can, tackling challenges, and building solid,
          functional solutions. We believe in the value of hard work and
          continuous improvement as we develop our expertise.
        </p>
        <h2 className="text-2xl font-bold mb-4">Meet Us</h2>
        <div className="grid gap-8 grid-cols-2">
          <div className="rounded-lg border border-gray-200 bg-white p-6 shadow-sm">
            <h3 className="text-xl font-semibold mb-2">Aleksa Perić</h3>
            <p className="text-gray-600 mb-4">Full Stack Developer</p>
            <div className="flex justify-center space-x-4">
              <a
                href="https://www.linkedin.com/in/aleksaperic02/"
                target="_blank"
              >
                <FaLinkedin className="w-12 h-12 my-text-gray" />
                <span className="sr-only">LinkedIn account Aleksa Perić</span>
              </a>
              <a href="https://github.com/aleksa1205" target="_blank">
                <FaGithub className="w-12 h-12 my-text-gray" />
              </a>
            </div>
          </div>

          <div className="rounded-lg border border-gray-200 bg-white p-6 shadow-sm">
            <h3 className="text-xl font-semibold mb-2">Jovan Cvetković</h3>
            <p className="text-gray-600 mb-4">Full Stack Developer</p>
            <div className="flex justify-center space-x-4">
              <a href="https://www.linkedin.com/in/cjovan02/" target="_blank">
                <FaLinkedin className="w-12 h-12 my-text-gray" />
                <span className="sr-only">
                  LinkedIn account Jovan Cvetković
                </span>
              </a>
              <a href="https://github.com/CJovan02" target="_blank">
                <FaGithub className="w-12 h-12 my-text-gray" />
              </a>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
};

export default AboutUs;
