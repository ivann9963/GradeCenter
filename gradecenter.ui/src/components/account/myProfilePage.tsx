import { AspNetUser } from "../../models/aspNetUser";
import { Container, Box, Typography, Paper, Tabs, Tab } from "@mui/material";
import { useEffect, useState } from "react";
import axios from "axios";
import Personal from "../profile-panel/personal-page";

export default function(){
     const [user, setUser] = useState<AspNetUser | null>(null);
     const [tabValue, setTabValue] = useState(0);
     const token = sessionStorage["jwt"];
     
     useEffect(() => {
        getLoggedUser();
      }, [])

     const getLoggedUser = () => {
         const url = `https://localhost:7273/api/Account/GetLoggedUser`;
         axios({
           method: "get",
           url: url,
           headers: {
             "Content-Type": "application/json",
             Authorization: `Bearer ${token}`,
           },
         }).then((res) => {
            const user = res.data;
            setUser(user);
            console.log(user);
         });
    };

    const handleTabChange = (event: React.ChangeEvent<{}>, newValue: number) => {
        console.log(newValue);
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
                <Typography><Personal profile={user} /></Typography>
              </Box>
            )}
            {tabValue === 1 && (
              <Box p={3}>
                <Typography></Typography>
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