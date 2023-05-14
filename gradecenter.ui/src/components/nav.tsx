import React, { useState } from "react";
import List from "@material-ui/core/List";
import ListItemText from "@material-ui/core/ListItemText";
import ListItem from "@material-ui/core/ListItem";
import HomeIcon from "@material-ui/icons/Home";
import ListItemIcon from "@material-ui/core/ListItemIcon";
import { School } from "@mui/icons-material";

export default function Nav() {
  return (
    <div>
      <nav>
        <List>
          {["Home", "Schools", "Class" ].map((text, index) => (
              <ListItem button key={text} component="a" href={'/' + text.toLowerCase()}>
                <ListItemIcon>
                  {
                    [<HomeIcon />, <School />, <HomeIcon />][index]
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
