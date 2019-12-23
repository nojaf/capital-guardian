import React, { useState } from "react";
import {
  NavbarToggler,
  Navbar,
  NavItem,
  Collapse,
  NavLink,
  Nav
} from "reactstrap";
import NavbarText from "reactstrap/lib/NavbarText";
import { A, usePath } from "hookrouter";

const Link = ({ route, children }) => {
  const path = usePath();
  return (
    <NavItem active={route === path}>
      <NavLink href={route} tag={A}>
        {children}
      </NavLink>
    </NavItem>
  );
};

const Navigation = () => {
  const [isOpen, setIsOpen] = useState(false);
  const toggle = () => setIsOpen(!isOpen);

  return (
    <Navbar color={"light"} light expand={"md"}>
      <NavbarToggler onClick={toggle} />
      <Collapse isOpen={isOpen} navbar>
        <Nav className="mr-auto" navbar>
          <Link route={"/"}>Home</Link>
          <Link route={"/overview"}>Overview per month</Link>
          <Link route={"/rules"}>Rules</Link>
        </Nav>
        <NavbarText>logout</NavbarText>
      </Collapse>
    </Navbar>
  );
};

export default Navigation;
