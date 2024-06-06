import axios from "axios";
import { useEffect, useState } from "react";
import { SelectField } from "../../common/inputs/SelectField";
import MapComponent from "../../common/map";
import { IDelivery, INovaPoshta, INovaPoshtaWarehouse } from "./types";

interface Props
{
    deliveryInfo: IDelivery;
    setDeliveryInfo: (value:IDelivery)=>void;
}

export const Novaposhta : React.FC<Props> = ({deliveryInfo, setDeliveryInfo}) => {
  const [novaposhta, setNovaposhta] = useState<INovaPoshta>({
    areas: [],
    cities: [],
    expires: new Date(),
  });
  const [warehouses, setWarehouses] = useState<INovaPoshtaWarehouse[]>([]);


  useEffect(() => {
      //console.log("Data novaposhta", localStorage.getItem("novaposhta"));
      //let novaposhta = localStorage.getItem("novaposhta");
     // if(novaposhta==null)
    //      novaposhta="";
    // let poshta = undefined;
    // try {
    //     const resultParse = JSON.parse(novaposhta);
    //     poshta=resultParse;
    //
    // }
    // catch{
    //     poshta=undefined;
    // }

    //if (poshta == undefined || poshta.expires > new Date()) {
      //  console.log("Find data Novaposhta")
      let today = new Date();
      let tomorrow = new Date();
      tomorrow.setDate(today.getDate() + 1);
      let novaposhtaOpt: INovaPoshta = {
        areas: [],
        cities: [],
        expires: tomorrow,
      };
      axios
        .post("https://api.novaposhta.ua/v2.0/json/", {
            apiKey: "63aa362a44e812e38243bd8fb803b606",
            modelName: "Address",
            calledMethod: "getAreas",
            methodProperties: {   }
        })
        .then((resp) => {
          novaposhtaOpt.areas = resp.data.data;
          axios
            .post("https://api.novaposhta.ua/v2.0/json/", {
                apiKey: "63aa362a44e812e38243bd8fb803b606",
                modelName: "Address",
                calledMethod: "getCities",
                methodProperties: {},

            })
            .then((resp) => {
              novaposhtaOpt.cities = resp.data.data;
                console.log("Set Data", resp.data);
              //localStorage.novaposhta = "sfsfd";//JSON.stringify(novaposhtaOpt);
              setNovaposhta(novaposhtaOpt);
            });
        });
    // } else {
    //   setNovaposhta(poshta);
    // }

    const delivery = localStorage.delivery
      ? JSON.parse(localStorage.delivery)
      : {
          area: "",
          city: "",
          warehouse: "",
        };
    setDeliveryInfo(delivery);
  }, []);

  useEffect(() => {
    if (deliveryInfo.city !== "") {
      axios
        .post("https://api.novaposhta.ua/v2.0/json/", {
          modelName: "Address",
          calledMethod: "getWarehouses",
          methodProperties: {
            CityName: deliveryInfo.city,
          },
          apiKey: "63aa362a44e812e38243bd8fb803b606",
        })
        .then((resp) => {
          const { data } = resp.data;
          setWarehouses(data);
          setDeliveryInfo({ ...deliveryInfo, warehouse: data[0].Description });
        });
    }
  }, [deliveryInfo.city, deliveryInfo.area]);

  const onChange = (e: any) => {
    const { value } = e.target;
    const object = { ...deliveryInfo, [e.target.name]: value };
    setDeliveryInfo(object);
    localStorage.delivery = JSON.stringify(object);
  };

  return (
    <>
      <div className="grid grid-cols-1 gap-6 mt-8 md:grid-cols-2">
        <div>
          <SelectField
            label={"Область"}
            value={
              deliveryInfo.area !== ""
                ? deliveryInfo.area
                : novaposhta.areas[0]?.Description
            }
            field={"area"}
            items={novaposhta.areas?.map((item: any) => {
              return {
                label: item!.Description,
                value: item!.Description,
              };
            })}
            onChange={(e) => {
              const { value } = e.target;
              const object = {
                ...deliveryInfo,
                area: value,
                city: novaposhta?.cities.filter(
                  (item) => item.AreaDescription == value
                )[0].Description,
              };
              setDeliveryInfo(object);
              localStorage.delivery = JSON.stringify(object);
            }}
          />
        </div>
        <div>
          <SelectField
            label={"Населений пункт"}
            value={
              deliveryInfo.city !== ""
                ? deliveryInfo.city
                : novaposhta.cities[0]?.Description
            }
            field={"city"}
            items={novaposhta.cities
              .filter(
                (item) =>
                  item.AreaDescription ===
                  (deliveryInfo.area !== ""
                    ? deliveryInfo.area
                    : novaposhta.areas[0]?.Description)
              )
              .map((item) => {
                return {
                  label: item.Description,
                  value: item.Description,
                };
              })}
            onChange={onChange}
          />
        </div>
        <div className="mb-6 col-span-2">
          <SelectField
            label={"Відділення"}
            value={deliveryInfo.warehouse}
            field={"warehouse"}
            items={warehouses.map((item) => {
              return {
                label: item.Description,
                value: item.Description,
              };
            })}
            onChange={onChange}
          />
        </div>
        <div className="h-96 col-span-2 -mt-6 mb-6">
          <MapComponent
            center={
              warehouses.filter(
                (item) => item.Description === deliveryInfo.warehouse
              )[0]
            }
            warehouses={warehouses}
          />
        </div>
      </div>
    </>
  );
};
