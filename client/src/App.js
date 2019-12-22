import React from "react";
import { ElmishCapture } from "./bin/Main";
import { Loader } from "./components";
import { MonthPage, NotFoundPage } from "./pages";
import { useRoutes } from "hookrouter";

const currentYear = new Date().getFullYear();
const currentMonth = new Date().getMonth() + 1;

const routes = {
  "/": () => <MonthPage year={currentYear} month={currentMonth} />
};

const App = () => {
  const routeResult = useRoutes(routes);
  return (
    <ElmishCapture loading={Loader}>
      {routeResult || <NotFoundPage />}
    </ElmishCapture>
  );
};

export default App;
