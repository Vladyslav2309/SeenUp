import { useEffect, useState } from "react";
import { Link, useLocation, useNavigate, useParams } from "react-router-dom";
import http from "../../../http";
import { useDispatch, useSelector } from "react-redux";
import { IProduct } from "../types";
import { ICart, ICartItem } from "../../common/basket/types";
import { setCart, setOpen } from "../../common/basket/CartReducer";
import Cookies from "js-cookie";
import Carousel from "../../common/carousel";

const ProductPage = () => {
  const [product, setProduct] = useState<IProduct>({
    name: "",
    price: "",
    description: "",
    category: "",
    decreasePercent: "",
    images: [],
    id: 1,
  });

  const { cart } = useSelector((store: any) => store.shoppingCart as ICart);

  const navigator = useNavigate();
  const location = useLocation();
  const dispatch = useDispatch();

  const { id } = useParams();

  const addItemToCart = () => {
    const index = cart.findIndex(
      (cartItem: ICartItem) => cartItem.id === parseInt(id || "0")
    );
    if (index === -1) {
      const updatedCart = [
        ...cart,
        {
          id: parseInt(id || "0"),
          name: product.name,
          category: product.category,
          price: parseInt(product.price || "0"),
          image: product.images[0],
          quantity: 1,
          decreasePercent: parseInt(product.decreasePercent || "0"),
        },
      ];
      if (Cookies.get("token")) {
        http.put("/api/account/basket", {
          productId: id,
          count: 1,
        });
      }
      dispatch(setCart(updatedCart));
    }
  };

  useEffect(() => {
    http
      .get("/api/products/id/" + id)
      .then((resp) => {
        setProduct(resp.data);
        if (localStorage.recently) {
          const recently = JSON.parse(localStorage.recently);
          localStorage.setItem(
            "recently",
            JSON.stringify([
              resp.data,
              ...recently.filter((item: any) => item.id !== resp.data.id),
            ])
          );
        } else localStorage.setItem("recently", JSON.stringify([resp.data]));
      })
      .catch(() => {
        navigator("/error404");
      });
  }, [location.pathname]);

  return (
    <div className="pt-6">
      <nav aria-label="Breadcrumb">
        <ol
          role="list"
          className="mx-auto flex max-w-2xl items-center space-x-2 px-4 sm:px-6 lg:max-w-7xl lg:px-8"
        >
          <li>
            <div className="flex items-center">
              <Link
                to={"/products?page=1"}
                className="mr-2 text-sm font-medium text-gray-900 dark:text-gray-400 dark:hover:text-gray-300"
              >
                Товари
              </Link>
              <svg
                width={16}
                height={20}
                viewBox="0 0 16 20"
                fill="currentColor"
                aria-hidden="true"
                className="h-5 w-4 text-gray-300"
              >
                <path d="M5.697 4.34L8.98 16.532h1.327L7.025 4.341H5.697z" />
              </svg>
            </div>
          </li>
          <li>
            <div className="flex items-center">
              <Link
                to={"/products?page=1&category=" + product?.category}
                className="mr-2 text-sm font-medium text-gray-900 dark:text-gray-400 dark:hover:text-gray-300"
              >
                {product?.category}
              </Link>
              <svg
                width={16}
                height={20}
                viewBox="0 0 16 20"
                fill="currentColor"
                aria-hidden="true"
                className="h-5 w-4 text-gray-300"
              >
                <path d="M5.697 4.34L8.98 16.532h1.327L7.025 4.341H5.697z" />
              </svg>
            </div>
          </li>
          <li className="text-sm">
            <Link
              to={""}
              aria-current="page"
              className="font-medium text-gray-500 hover:text-gray-600 dark:text-gray-300 dark:hover:text-gray-200"
            >
              {product!.name.length > 37
                ? product!.name.substring(0, 37) + "..."
                : product!.name}
            </Link>
          </li>
        </ol>
      </nav>

      <div className="mx-auto max-w-2xl px-4 pt-10 pb-16 sm:px-6 lg:grid lg:max-w-7xl lg:grid-cols-3 lg:grid-rows-[auto,auto,1fr] lg:gap-x-8 lg:px-8 lg:pt-16 lg:pb-24">
        <div className="lg:col-span-2 lg:border-r lg:border-gray-200 dark:lg:border-gray-600 lg:pr-8">
          <h1 className="text-2xl font-bold tracking-tight text-gray-900 dark:text-gray-200 sm:text-3xl mb-5">
            {product?.name}
          </h1>
          <Carousel images={product.images} />
        </div>

        <div className="mt-4 lg:row-span-3 lg:mt-0">
          {product.decreasePercent ? (
            <>
              <p className="text-lg tracking-tight text-gray-900 dark:text-gray-300 line-through">
                {product?.price.toLocaleString()} ₴
              </p>
              <p className="text-3xl tracking-tight text-red-500">
                {(
                  parseFloat(product?.price) -
                  (parseFloat(product?.price) *
                    parseFloat(product?.decreasePercent)) /
                    100
                ).toLocaleString()}{" "}
                ₴
              </p>
            </>
          ) : (
            <p className="text-3xl tracking-tight text-gray-900 dark:text-gray-300">
              {product?.price.toLocaleString()} ₴
            </p>
          )}

          {cart.findIndex(
            (cartItem: ICartItem) => cartItem.id === parseInt(id || "0")
          ) === -1 ? (
            <button
              type="submit"
              onClick={addItemToCart}
              className="mt-10 flex w-full items-center justify-center rounded-md border border-transparent bg-indigo-600 py-3 px-8 text-base font-medium text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
            >
              Додати в кошик
            </button>
          ) : (
            <button
              type="submit"
              onClick={() => dispatch(setOpen(true))}
              className="mt-10 flex w-full items-center justify-center rounded-md border border-transparent bg-green-600 py-3 px-8 text-base font-medium text-white hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
            >
              В корзині
            </button>
          )}
        </div>

        <div className="py-10 lg:col-span-2 lg:col-start-1 lg:border-r lg:border-gray-200 dark:lg:border-gray-600 lg:pt-6 lg:pb-16 lg:pr-8">
          <div>
            <div
              className="prose text-black dark:text-gray-200"
              dangerouslySetInnerHTML={{ __html: product!.description }}
            />
          </div>
        </div>
      </div>
    </div>
  );
};
export default ProductPage;
