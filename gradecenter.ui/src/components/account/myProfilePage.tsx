import { AspNetUser } from "../../models/aspNetUser";
import { useParams } from "react-router-dom";
import { Container, Box, Typography, Paper, Tabs, Tab } from "@mui/material";
import { useEffect, useState } from "react";
import axios from "axios";
import Personal from "../profile-panel/personal-page";
import Grades from "../profile-panel/grades-page";
import requests from "../../requests";
import Attendance from "../profile-panel/attendance-page";

export default function(){
     let { profileId } = useParams();
     const [user, setUser] = useState<AspNetUser | null>(null);
     const [tabValue, setTabValue] = useState(0);
     
     useEffect(() => {
        profileId != undefined? getUserById(profileId) : getLoggedUser();
     }, [])

     const getUserById = (profileId: any) => {
        requests.getUserById(profileId)
        .then((res) => {
           const user = res.data;
           setUser(user);
        });
     }

     const getLoggedUser = () => {
         requests.getLoggedUser().then((res) => {
            const user = res.data;
            setUser(user);
            console.log(user);
         });
    };

    const handleTabChange = (event: React.ChangeEvent<{}>, newValue: number) => {
        setTabValue(newValue);
    };

    return(
     <Container style={{ padding: "4em 6em", marginLeft: 270 }}>
        {
        user ? (
          <Box>
          <br />
          <Typography variant="h4" marginBottom="1.5em">My Profile</Typography>
          <Paper elevation={10}>
            <Tabs value={tabValue} onChange={handleTabChange}>
              <Tab label="Personal" />
              <Tab label="Family" />
              <Tab label="Attendance" />
              <Tab label="Grades" />
            </Tabs>
            
            {tabValue === 0 && (
              <Box p={3}>
                <Typography>
                    <Personal profile={user} />
                </Typography>
              </Box>
            )}
            {tabValue === 2 && (
              <Box p={3}>
              <Typography>
                  <Attendance profile={user} />
              </Typography>
            </Box>
            )}
            {tabValue === 3 && (
              <Box p={3}>
                <Typography>
                    <Grades profile={user} />
                </Typography>
              </Box>
            )}
          </Paper>
        </Box>
        )
        :(
            <Typography variant="h4">Loading...</Typography>
        )}
      </Container>
    )
}