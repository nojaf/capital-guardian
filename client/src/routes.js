import React from "react";
import { HomePage, MonthPage, OverviewPage, SpreadPage } from "./pages";

const routes = {
  "/": () => <HomePage />,
  "/overview": () => <OverviewPage />,
  "/:year/:month": ({ year, month }) => (
    <MonthPage year={parseInt(year)} month={parseInt(month)} />
  ),
  "/spread": () => <SpreadPage />,
};

export default routes;
