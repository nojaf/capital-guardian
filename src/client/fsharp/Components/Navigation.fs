module CapitalGuardian.Client.Components.Navigation

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Feliz
open CapitalGuardian.Client.Hooks
open CapitalGuardian.Client.Styles

[<ReactComponent()>]
let Navigation () =
    let showBrand = useXsOrSm ()
    let navClasses =
        [
            Bootstrap.Navbar
            if showBrand then Bootstrap.NavbarDark else Bootstrap.NavbarLight
            if showBrand then Bootstrap.BgDark else Bootstrap.BgLight
            Bootstrap.NavbarExpandMd
        ]

    nav [ classNames navClasses ] [
        if showBrand then
            a [classNames [ Bootstrap.NavbarBrand; Bootstrap.MeAuto; Bootstrap.TextPrimary ]; Href "#"] [str "Capital Guardian"]
        button [ClassName Bootstrap.NavbarToggler] [
            span [ClassName Bootstrap.NavbarTogglerIcon] []
        ]
        (*
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
      <span class="navbar-toggler-icon"></span>
    </button>
        *)
    ]

    (*
import React, { useState } from "react";
import {
  NavbarToggler,
  Navbar,
  NavItem,
  Collapse,
  NavLink,
  Nav,
  NavbarBrand,
} from "reactstrap";
import NavbarText from "reactstrap/lib/NavbarText";
import { A, usePath } from "hookrouter";
import { useXsOrSm } from "../hooks/breakpoints";
import { useAuth0 } from "../react-auth0-spa";
import { currentDomain } from "../utils";

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
  const { logout } = useAuth0();
  const logoutHandler = () => {
    logout({ returnTo: currentDomain });
  };

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
          <Link route={"/spread"}>Spread over months</Link>
        </Nav>
        <NavbarText className={"pointer"} onClick={logoutHandler}>
          logout
        </NavbarText>
      </Collapse>
    </NavbarInner>
  );
};

export default Navigation;

    *)