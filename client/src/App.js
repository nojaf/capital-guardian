import React from "react";
import { ElmishCapture } from "./bin/Main";
import { Header, Loader, Navigation, ToastContainer } from "./components";
import { HomePage, MonthPage, NotFoundPage, OverviewPage } from "./pages";
import { useRoutes } from "hookrouter";
import { Container } from "reactstrap";
import { useXsOrSm } from "./hooks/breakpoints";

const routes = {
  "/": () => <HomePage />,
  "/overview": () => <OverviewPage />,
  "/:year/:month": ({ year, month }) => (
    <MonthPage year={parseInt(year)} month={parseInt(month)} />
  )
};

const App = () => {
  const small = useXsOrSm();
  const routeResult = useRoutes(routes);
  return (
    <ElmishCapture loading={Loader}>
      {small && <Navigation />}
      <Container>
        {!small && <Header />}
        {!small && <Navigation />}
        {routeResult || <NotFoundPage />}
      </Container>
      <ToastContainer />
    </ElmishCapture>
  );
};

export default App;
