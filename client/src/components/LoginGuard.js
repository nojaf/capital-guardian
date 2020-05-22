import React, { useEffect, useState } from "react";
import Loader from "./Loader";
import { useAuth0 } from "../react-auth0-spa";
import PropTypes from "prop-types";

const LoginGuard = ({ render }) => {
  const { isAuthenticated, loginWithRedirect, getTokenSilently } = useAuth0();
  const [token, setToken] = useState(null);
  useEffect(() => {
    if (!isAuthenticated) {
      return loginWithRedirect();
    } else {
      getTokenSilently().then((token) => setToken(token));
    }
  }, [isAuthenticated, loginWithRedirect, getTokenSilently]);

  return isAuthenticated && token ? render(token) : <Loader />;
};

LoginGuard.propTypes = {
  render: PropTypes.func,
};

export default LoginGuard;
