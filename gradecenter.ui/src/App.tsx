import { BrowserRouter, Routes, Route } from "react-router-dom";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

import Drawer from "./components/drawer";
import Register from "./components/account/registerPage";
import Login from "./components/account/loginPage";
import Home from "./components/home/home"; 
import Schools from "./components/school/schools-list";
import SchoolDetails from "./components/school/school-details";
import AdminPanel from "./components/admin-panel/admin-panel";
import MySchedulle from "./components/my-schedulle/mySchedulle";
import MyProfilePage from "./components/account/myProfilePage";

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
        <Route path="/" element={getComponent(<Home />)} />
        <Route path="/home" element={getComponent(<Home />)} />
        <Route path="/register" element={<Register />} />
        <Route path="/login" element={<Login />} />
        <Route path="/schools" element={getComponent(<Schools />)} />
        <Route path="/school-details/:schoolId" element={getComponent(<SchoolDetails />)} />
        <Route path="/admin-panel" element={getComponent(<AdminPanel />)} />
        <Route path="/my-schedulle" element={getComponent(<MySchedulle />)} />
        <Route path="/profile" element={getComponent(<MyProfilePage />)} />
      </Routes>
    </BrowserRouter>
  );
}