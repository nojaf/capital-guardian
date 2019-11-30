import React from "react";
import { ElmishCapture } from "./bin/Main";
import { Loader } from "./components";
import HomePage from "./pages/HomePage";

const App = () => {
  return (
    <ElmishCapture loading={Loader}>
      <HomePage />
    </ElmishCapture>
  );
};

export default App;
