import React from "react";
import { Spinner } from "reactstrap";

const Loader = () => {
  return (
    <div className={"vh-100 d-flex align-items-center justify-content-center"}>
      <Spinner color="primary" />
    </div>
  );
};

export default Loader;
