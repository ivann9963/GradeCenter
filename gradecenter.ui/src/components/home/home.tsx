import React, { useEffect, useState } from "react";
import Schools from "../school/schools-list";
import { AspNetUser, UserRoles } from "../../models/aspNetUser";
import requests from "../../requests";
import MyClass from "../school/my-class-page";


const Home = () => {
  return <Schools />;
};

export default Home;
