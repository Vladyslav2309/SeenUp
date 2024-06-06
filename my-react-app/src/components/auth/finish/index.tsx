import { ArrowLeftOnRectangleIcon } from "@heroicons/react/20/solid";
import classNames from "classnames";
import { useFormik } from "formik";
import Cookies from "js-cookie";
import jwt from "jwt-decode";
import { useEffect } from "react";
import { useDispatch } from "react-redux";
import { useNavigate, useSearchParams } from "react-router-dom";
import http from "../../../http";
import { setCart } from "../../common/basket/CartReducer";
import { IBasketResponce, ICartItem } from "../../common/basket/types";
import { setUser } from "../AuthReducer";
import { IGoogleJWT, IGoogleRegisterUser } from "./types";
import { GoogleRegisterSchema } from "./validation";
import logo from "../../../assets/logo.svg";

const GoogleRegistration = () => {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const [searchParams] = useSearchParams();
  const token = searchParams.get("token");

  useEffect(() => {
    if (!token) {
      navigate("/error404");
      return;
    }

    const decodeJWT = jwt<IGoogleJWT>(token);

    if (new Date(decodeJWT.exp * 1000) < new Date()) {
      navigate("/error404");
      return;
    }

    formik.setValues({
      firstName: decodeJWT.given_name || "",
      lastName: "",
      token: token,
      image: decodeJWT.picture || "",
    });
  }, [token, navigate]);

  const onSubmitFormik = async (values: IGoogleRegisterUser) => {
    try {
      const result = await http.post("/api/auth/google/register", values);
      const { token } = result.data;

      const decodedToken = jwt(token);
      const expirationDate = new Date(decodedToken.exp * 1000);
      Cookies.set("token", token, { expires: expirationDate });
      http.defaults.headers.common["Authorization"] = `Bearer ${token}`;

      const cartFromLocalStorage = localStorage.getItem("cart");
      if (cartFromLocalStorage) {
        const cart = JSON.parse(cartFromLocalStorage) as ICartItem[];
        cart.forEach((item) => {
          http.put("/api/account/basket", {
            productId: item.id,
            count: item.quantity,
          });
        });
      }

      http.get<IBasketResponce>("/api/account/basket").then((resp) => {
        const { data } = resp;
        const list = data.list.map((item) => ({
          id: item.product.id,
          name: item.product.name,
          category: item.product.category,
          price: parseFloat(item.product.price),
          image: item.product.images[0],
          quantity: item.count,
          decreasePercent: parseInt(item.product.decreasePercent),
        }));

        dispatch(setCart(list));
      });

      dispatch(
          setUser({
            isAuth: true,
            name: decodedToken?.name,
            email: decodedToken?.email,
            image: decodedToken?.image,
            roles: decodedToken?.roles,
            emailConfirmed: decodedToken?.emailConfirmed.toLowerCase() === "true",
          })
      );

      navigate("/");
    } catch (error) {
      formik.setFieldError("token", "Помилка, попробуйте зайти в свій Google аккаунт, ще раз!");
    }
  };

  const formik = useFormik({
    initialValues: {
      firstName: "",
      lastName: "",
      token: "",
      image: "",
    },
    validationSchema: GoogleRegisterSchema,
    onSubmit: onSubmitFormik,
  });

  const { values, errors, touched, handleSubmit, handleChange } = formik;

  return (
      <section className="justify-center w-full m-2 max-w-lg mx-auto overflow-hidden bg-white rounded-lg shadow-lg dark:bg-gray-800 lg:max-w-7xl">
        <div className="flex justify-center">
          <div className="justify-center items-center w-full max-w-3xl p-8 mx-auto lg:px-12 lg:w-3/5">
            <div className="w-full">
              <div className="flex justify-center mx-auto">
                <img className="w-auto h-16" src={logo} alt="" />
              </div>
              <h1 className="text-2xl font-semibold tracking-wider text-gray-800 capitalize dark:text-white">
                Завершення реєстрації.
              </h1>
              <p className="mt-4 text-gray-500 dark:text-gray-400">
                Перевірте правильність, наданої інформації від Google, та натискайте Продовжити. За потреби Ви можете змінити їх.
              </p>
              {touched.token && errors.token && (
                  <p className="mt-3 text-base text-red-400">{errors.token}</p>
              )}
              <form className="grid grid-cols-1 gap-6 mt-8 md:grid-cols-2" onSubmit={handleSubmit}>
                <div>
                  <label htmlFor="firstName" className="block mb-2 text-sm text-gray-600 dark:text-gray-200">
                    Ім'я
                  </label>
                  <input
                      type="text"
                      placeholder="Ім'я"
                      id="firstName"
                      onChange={handleChange}
                      value={values.firstName}
                      name="firstName"
                      className={classNames(
                          "block w-full px-5 py-3 mt-2 text-gray-700 placeholder-gray-400 bg-white border border-gray-200 rounded-lg dark:placeholder-gray-600 dark:bg-gray-900 dark:text-gray-300 dark:border-gray-700 focus:border-blue-400 dark:focus:border-blue-400 focus:ring-blue-400 focus:outline-none focus:ring focus:ring-opacity-40",
                          {
                            "dark:focus:border-red-300 focus:border-red-400 focus:ring-red-300 dark:border-red-400 border-red-400": touched.firstName && errors.firstName,
                            "dark:focus:border-green-300 focus:border-green-400 focus:ring-green-300 dark:border-green-400 border-green-400": touched.firstName && !errors.firstName,
                          }
                      )}
                  />
                  {touched.firstName && errors.firstName && (
                      <p className="mt-3 text-xs text-red-400">{errors.firstName}</p>
                  )}
                </div>
                <div>
                  <label htmlFor="lastName" className="block mb-2 text-sm text-gray-600 dark:text-gray-200">
                    Прізвище
                  </label>
                  <input
                      type="text"
                      placeholder="Прізвище"
                      id="lastName"
                      onChange={handleChange}
                      value={values.lastName}
                      name="lastName"
                      className={classNames(
                          "block w-full px-5 py-3 mt-2 text-gray-700 placeholder-gray-400 bg-white border border-gray-200 rounded-lg dark:placeholder-gray-600 dark:bg-gray-900 dark:text-gray-300 dark:border-gray-700 focus:border-blue-400 dark:focus:border-blue-400 focus:ring-blue-400 focus:outline-none focus:ring focus:ring-opacity-40",
                          {
                            "dark:focus:border-red-300 focus:border-red-400 focus:ring-red-300 dark:border-red-400 border-red-400": touched.lastName && errors.lastName,
                            "dark:focus:border-green-300 focus:border-green-400 focus:ring-green-300 dark:border-green-400 border-green-400": touched.lastName && !errors.lastName,
                          }
                      )}
                  />
                  {touched.lastName && errors.lastName && (
                      <p className="mt-3 text-xs text-red-400">{errors.lastName}</p>
                  )}
                </div>
                <div className="col-span-2">
                  <button
                      type="submit"
                      className="w-full px-6 py-3 text-sm font-medium tracking-wide text-white capitalize transition-colors duration-300 transform bg-blue-500 rounded-lg hover:bg-blue-400 focus:outline-none focus:ring focus:ring-blue-300 focus:ring-opacity-50"
                  >
                    Продовжити
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      </section>
  );
};

export default GoogleRegistration;
