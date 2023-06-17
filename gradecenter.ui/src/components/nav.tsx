import React, { useEffect, useState } from "react";
import List from "@material-ui/core/List";
import ListItemText from "@material-ui/core/ListItemText";
import ListItem from "@material-ui/core/ListItem";
import HomeIcon from "@material-ui/icons/Home";
import ListItemIcon from "@material-ui/core/ListItemIcon";
import PeopleIcon from '@mui/icons-material/People';
import CalendarMonthIcon from '@mui/icons-material/CalendarMonth';
import DonutSmallIcon from '@mui/icons-material/DonutSmall';
import { AspNetUser, UserRoles } from "../models/aspNetUser";
import requests from "../requests";

export default function Nav() {
  const [user, setUser] = useState<AspNetUser | null>(null);
  const userRole = user?.userRole as number;
  
  useEffect(() => {
    requests.getLoggedUser().then((res) => {
       const user = res.data;
       setUser(user);
    });
  }, []);

  const menuItems: string[] = [];

  if(userRole == UserRoles.Admin) {
    menuItems.splice(0, 0, "Home");
    menuItems.splice(3, 0, "Statistics");
  }
  
  if(userRole == UserRoles.Teacher || userRole == UserRoles.Student || userRole == UserRoles.Parent) {
    menuItems.splice(0, 0, "My Class");
    menuItems.splice(1, 0, "My schedulle");
  }

  return (
    <div>
      <nav>
        <List>
          {menuItems.map((text, index) => (
              <ListItem button key={text} component="a" href={'/' + text.replace(' ', '-').toLowerCase()}>
                <ListItemIcon>
                  {
                    [<HomeIcon />, <PeopleIcon />, <CalendarMonthIcon />, <DonutSmallIcon /> ][index]
                  }
                </ListItemIcon>
                <ListItemText primary={text} />
              </ListItem>
          ))}
        </List>
      </nav>
    </div>
  );
}
