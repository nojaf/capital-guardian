import React from 'react';
import ReactDOM from 'react-dom';
import { Auth0Provider, withAuthenticationRequired  } from '@auth0/auth0-react';
import App from './bin/App';
import './style.sass';

const SecureApp = withAuthenticationRequired(App);

ReactDOM.render(
  <React.StrictMode>
    <Auth0Provider
      domain={import.meta.env.SNOWPACK_PUBLIC_AUTH0_DOMAIN}
      clientId={import.meta.env.SNOWPACK_PUBLIC_AUTH0_CLIENT_ID}
      audience={import.meta.env.SNOWPACK_PUBLIC_AUTH0_AUDIENCE}
      redirectUri={window.location.origin}
      useRefreshTokens
      scope={"openid profile offline_access"}
    >
      {/* <LoginGuard render={(token) => {
        return <div>
          <App />
          {token}
        </div>
      }} /> */}
      <SecureApp />
    </Auth0Provider>
  </React.StrictMode>,
  document.getElementById('root'),
);

// Hot Module Replacement (HMR) - Remove this snippet to remove HMR.
// Learn more: https://www.snowpack.dev/concepts/hot-module-replacement
if (import.meta.hot) {
  import.meta.hot.accept();
}
