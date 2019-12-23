import React from "react";
import { ElmishCapture } from "./bin/Main";
import { Header, Loader, Navigation } from "./components";
import { MonthPage, NotFoundPage, OverviewPage } from "./pages";
import { useRoutes } from "hookrouter";
import { Container } from "reactstrap";

const currentYear = new Date().getFullYear();
const currentMonth = new Date().getMonth() + 1;

const routes = {
  "/": () => <MonthPage year={currentYear} month={currentMonth} />,
  "/overview": () => <OverviewPage />
};

const App = () => {
  const routeResult = useRoutes(routes);
  return (
    <ElmishCapture loading={Loader}>
      <Container>
        <Header />
        <Navigation />
        {routeResult || <NotFoundPage />}
      </Container>
    </ElmishCapture>
  );
};

export default App;
