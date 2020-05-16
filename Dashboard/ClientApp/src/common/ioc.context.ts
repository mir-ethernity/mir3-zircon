import { Container } from "aurelia-dependency-injection";
import React from "react";

const IoCContext = React.createContext(new Container());

export default IoCContext;