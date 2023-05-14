import { BrowserRouter, Routes, Route } from "react-router-dom";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

import Drawer from "./components/drawer";
import Register from "./components/account/registerPage";
import Login from "./components/account/loginPage";
import Home from "./components/home/home"; // Import the Home component
import Schools from "./components/school/schoolPage";

export default function App() {
  const getComponent = (component: JSX.Element) => {
    const jwt: String = sessionStorage['jwt'];

    if(jwt && jwt.length > 10) {
      return component;
    } else {
      return <Login />
    }
  }

  return (
    <BrowserRouter>
      <Drawer />
      <Routes>
        <Route path="/register" element={<Register />} />
        <Route path="/login" element={<Login />} />
        <Route path="/home" element={getComponent(<Home />)} /> {/* Add the new route */}
        <Route path="/" element={getComponent(<Home />)} /> {/* Add the new route */}
        <Route path="/schools" element={getComponent(<Schools />)} />
      </Routes>
    </BrowserRouter>
  );
}