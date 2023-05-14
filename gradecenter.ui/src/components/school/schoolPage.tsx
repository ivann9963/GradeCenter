import { Container } from "@mui/system";
import { Card, CardContent, CardActions, Typography, Button, Grid } from "@mui/material";
import LocationCityIcon from '@mui/icons-material/LocationCity';
import GroupIcon from '@mui/icons-material/Group';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import {useEffect, useState} from "react";
import axios from "axios";
import React from "react";


const url = `https://localhost:7273/api/School/Read`;
var schools = [];
axios({
    method: "get",
    url: url,
    headers: {
      "Content-Type": "text/plain;charset=utf-8"
    },
  }).then((res) => {
        schools = res.data;
  });

export default function Schools(){
    const url = `https://localhost:7273/api/School/Read`;
    const [schools, setSchools] = useState([]);
    
    useEffect(() => {
        getAllSchools();
    }, [])

    const getAllSchools = () => {
        axios({
            method: "get",
            url: url,
            headers: {
              "Content-Type": "text/plain;charset=utf-8"
            },
          }).then((res) => {
                setSchools(res.data);
          });
    }

    return(
        <Container>
            <h1 style={{textAlign: "center"}}>Schools</h1>
            <Grid sx={{ flexGrow: 1 }} container spacing={2} columns={{ xs: 4, sm: 8, md: 12 }}>
                {schools.map(school => {
                    return(
                        <Grid item xs={4}>
                            <Card>
                                 <CardContent>
                                      <Typography gutterBottom variant="h5" component="div">
                                        {school["name"]}
                                        <LocationCityIcon style={{marginLeft: "1.5%"}} />
                                      </Typography>
                                      <Typography variant="body2" color="text.secondary">
                                            {school["address"]}
                                      </Typography>
                                </CardContent>
                                <CardActions>
                                      <Button variant="outlined">
                                            Students <GroupIcon/>
                                      </Button>
                                      <Button variant="outlined" color={"warning"}>
                                            Edit <EditIcon/>
                                      </Button>
                                      <Button variant="outlined" color={"error"}>
                                            Delete <DeleteIcon/>
                                      </Button>
                                </CardActions>
                            </Card>
                        </Grid>
                    );
                })
                }
            </Grid>
        </Container>
    )
}