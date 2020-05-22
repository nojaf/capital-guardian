import { navigate } from "hookrouter";
import { currentDomain } from "./utils";

const auth0Config = {
  domain: process.env.REACT_APP_AUTH0_DOMAIN,
  client_id: process.env.REACT_APP_AUTH0_CIENT_ID,
  audience: process.env.REACT_APP_AUTH0_AUDIENCE,
  redirect_uri: `${currentDomain}/oauth`,
  scope: process.env.REACT_APP_AUTH0_SCOPE,
  onRedirectCallback: () => navigate("/"),
};

export default auth0Config;
