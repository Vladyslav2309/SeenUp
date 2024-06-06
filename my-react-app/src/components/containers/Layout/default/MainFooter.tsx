import { Link } from "react-router-dom";
import logo from "../../../../assets/logo.svg";

export const MainFooter = () => {
  return (
    <>
      <footer className="bg-white dark:bg-gray-900">
        <div className="container flex flex-col items-center justify-center mx-auto space-y-4 sm:space-y-0 sm:flex-row">
          {/*<Link to="/">*/}
          {/*  <img className="w-auto h-8 my-4" src={logo} alt="" />*/}
          {/*</Link>*/}

          <p className="text-sm text-gray-600 dark:text-gray-300 pb-4">
            IT Step Lutsk 2024
          </p>

          <div className="flex">

          </div>
        </div>
      </footer>
    </>
  );
};
