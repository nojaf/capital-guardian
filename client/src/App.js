import React from "react";
import { ElmishCapture } from "./bin/Main";
import { Header, Navigation, ToastContainer, LoginGuard } from "./components";
import { NotFoundPage } from "./pages";
import { useRoutes } from "hookrouter";
import { Container } from "reactstrap";
import { useXsOrSm } from "./hooks/breakpoints";
import { Auth0Provider } from "./react-auth0-spa";
import routes from "./routes";
import auth0Config from "./auth0Config";

const App = () => {
  const small = useXsOrSm();
  const routeResult = useRoutes(routes);

  return (
    <Auth0Provider {...auth0Config}>
      <LoginGuard
        render={token => {
          return (
            <ElmishCapture token={token}>
              {small && <Navigation />}
              <Container>
                {!small && <Header />}
                {!small && <Navigation />}
                {routeResult || <NotFoundPage />}
              </Container>
              <ToastContainer />
            </ElmishCapture>
          );
        }}
      ></LoginGuard>
    </Auth0Provider>
  );
};

export default App;
