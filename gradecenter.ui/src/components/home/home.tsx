import React, { useEffect, useState } from "react";
import Schools from "../school/schools-list";
import { AspNetUser, UserRoles } from "../../models/aspNetUser";
import requests from "../../requests";
import MyClass from "../school/my-class-page";


const Home = () => {
  const [user, setUser] = useState<AspNetUser | null>(null);
  
  useEffect(() => {
    requests.getLoggedUser().then((res) => {
       const user = res.data;
       setUser(user);
    });
  }, []);

  const component: any = (): any => {
    if(user?.userRole as number == UserRoles.Admin) {
      return <Schools />
    } else {
      return <MyClass />
    }
  }

  return component();
};

export default Home;
