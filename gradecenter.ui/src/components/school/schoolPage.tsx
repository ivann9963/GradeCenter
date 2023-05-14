import { Container } from "@mui/system";
import { Card, CardContent, CardActions, Typography, Button, Grid, Box } from "@mui/material";
import LocationCityIcon from "@mui/icons-material/LocationCity";
import GroupIcon from "@mui/icons-material/Group";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import { useEffect, useState } from "react";
import axios from "axios";
import React from "react";
import IconButton from "@material-ui/core/IconButton";
import CardHeader from "@mui/material/CardHeader";
import Divider from "@material-ui/core/Divider";

const url = `https://localhost:7273/api/School/Read`;
var schools = [];
axios({
  method: "get",
  url: url,
  headers: {
    "Content-Type": "text/plain;charset=utf-8",
  },
}).then((res) => {
  schools = res.data;
});

export default function Schools() {
  const [schools, setSchools] = useState([]);

  useEffect(() => {
    getAllSchools();
  }, []);

  const getAllSchools = () => {
    const url = `https://localhost:7273/api/School/Read`;

    axios({
      method: "get",
      url: url,
      headers: {
        "Content-Type": "text/plain;charset=utf-8",
      },
    }).then((res) => {
      setSchools(res.data);
    });
  };

  const onDelete = (schoolName: string): void => {
    const url = `https://localhost:7273/api/School/Delete`;
    const token = sessionStorage['jwt'];

    console.log(token);

    axios({
      method: "delete",
      url: url,
      params: {
        name: schoolName,
      },
      headers: {
        "Content-Type": "text/plain;charset=utf-8",
        "Authorization": `Bearer ${token}` 
      },
    }).then((res) => {
      getAllSchools();
    }).catch((error) => {
        console.error(error);
    });
  };

  return (
    <Container style={{ padding: "7em 6em" }}>
      <Grid container spacing={3} columns={{ xs: 12, sm: 8, md: 12 }} marginLeft={10}>
        {schools.map((school, index) => (
          <Grid item xs={12} sm={6} md={4} key={index}>
            <Card elevation={18}>
              <Box p={1.5}>
                <Box display="flex" justifyContent="space-between" alignItems="center">
                  <Typography variant="h5">
                    <LocationCityIcon color="primary" />
                    {school["name"]}
                  </Typography>
                  <Box>
                    <IconButton color="default" onClick={() => onDelete(school["name"])}>
                      <DeleteIcon />
                    </IconButton>
                    <IconButton color="default">
                      <EditIcon />
                    </IconButton>
                  </Box>
                </Box>
                <Typography variant="body2" color="text.secondary">
                  {school["address"]}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  150 Students . 300 graduated
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  Economics
                </Typography>
              </Box>
              <Divider />
              <CardActions disableSpacing>
                <Button>Enroll</Button>
                <Button>Withdraw</Button>
              </CardActions>
            </Card>
          </Grid>
        ))}
      </Grid>
    </Container>
  );
}
