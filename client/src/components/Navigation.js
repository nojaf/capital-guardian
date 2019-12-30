import React, { useState } from "react";
import {
  NavbarToggler,
  Navbar,
  NavItem,
  Collapse,
  NavLink,
  Nav,
  NavbarBrand
} from "reactstrap";
import NavbarText from "reactstrap/lib/NavbarText";
import { A, usePath } from "hookrouter";
import { useXsOrSm } from "../hooks/breakpoints";

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
  const showBrand = useXsOrSm();

  const NavbarInner = ({ children }) =>
    showBrand ? (
      <Navbar color={"dark"} dark expand={"md"}>
        {children}
      </Navbar>
    ) : (
      <Navbar color={"light"} light expand={"md"}>
        {children}
      </Navbar>
    );

  return (
    <NavbarInner>
      {showBrand && (
        <NavbarBrand href="/" tag={A} className="mr-auto text-primary">
          Capital Guardian
        </NavbarBrand>
      )}
      <NavbarToggler onClick={toggle} />
      <Collapse isOpen={isOpen} navbar>
        <Nav className="mr-auto text-primary" navbar>
          <Link route={"/"}>Home</Link>
          <Link route={"/overview"}>Overview per month</Link>
          <Link route={'/spread'}>Spread over months</Link>
        </Nav>
        <NavbarText>logout</NavbarText>
      </Collapse>
    </NavbarInner>
  );
};

export default Navigation;
