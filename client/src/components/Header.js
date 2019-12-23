import React from "react";
import { Jumbotron } from "reactstrap";

const Header = () => {
  return (
    <Jumbotron>
      <div className="d-flex align-items-start">
        <img src="/logo.png" height={"100px"} alt="" className={"mt-3"} />
        <div className={"flex-grow-1"}>
          <h1 className="display-3 text-primary text-shadow">
            Capital Guardian
          </h1>
          <p className="lead px-2">Protector of prosperousness</p>
        </div>
      </div>
    </Jumbotron>
  );
};

export default Header;
