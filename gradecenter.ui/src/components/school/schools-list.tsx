import { Container } from "@mui/system";
import { Card, CardActions, Typography, Button, Grid, Box } from "@mui/material";
import LocationCityIcon from "@mui/icons-material/LocationCity";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import { useEffect, useState } from "react";
import axios from "axios";
import React from "react";
import IconButton from "@material-ui/core/IconButton";
import Divider from "@material-ui/core/Divider";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import TextField from "@mui/material/TextField";
import { useNavigate } from "react-router-dom";
import { Link } from "react-router-dom";
import { makeStyles } from "@material-ui/core/styles";


const useStyles = makeStyles((theme) => ({
  card: {
    "&:hover": {
      boxShadow: "0 0 11px rgba(0, 114, 255, 10)", // This will make the card "light up" when hovered over
    },
  },
  link: {
    textDecoration: "none",
    color: "inherit",
    "&:hover": {
      textDecoration: "underline",
    },
  },
  dialogPaper: {
    backgroundColor: 'rgba(0, 0, 0, 0.5)', // Adjust this to your liking
  },
  components: {
    MuiBackdrop: {
      styleOverrides: {
        root: {
          backgroundColor: 'rgba(0, 0, 0, 0.5)', // Adjust this to your liking
        },
      },
    },
  }
}));

export default function Schools() {
  const [schools, setSchools] = useState([]);
  const navigation = useNavigate();
  const classes = useStyles();

  useEffect(() => {
    getAllSchools();
  }, []);

  const getAllSchools = () => {
    const url = `https://localhost:7273/api/School/GetAllSchools`;

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
    const token = sessionStorage["jwt"];

    axios({
      method: "delete",
      url: url,
      params: {
        name: schoolName,
      },
      headers: {
        "Content-Type": "text/plain;charset=utf-8",
        Authorization: `Bearer ${token}`,
      },
    })
      .then((res) => {
        getAllSchools();
      })
      .catch((error) => {
        console.error(error);
      });
  };

  const [open, setOpen] = useState(false);
  const [selectedSchool, setSelectedSchool] = useState({ name: "", address: "" });

  const handleClickOpen = (school: React.SetStateAction<{ name: string; address: string }>) => {
    setSelectedSchool(school);
    setOpen(true);
  };

  const handleClose = (event: any) => {
    event.stopPropagation();
    setOpen(false);
  };

  const handleEdit = () => {
    const token = sessionStorage["jwt"];

    axios
      .put(`https://localhost:7273/api/School/Update`, selectedSchool, {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      })
      .then(() => {
        getAllSchools();
        handleClose(null);
      })
      .catch((error) => console.error(error));
  };

  const [createDialogOpen, setCreateDialogOpen] = useState(false);
  const [newSchool, setNewSchool] = useState({ name: "", address: "" });

  const handleCreateOpen = () => {
    setCreateDialogOpen(true);
  };

  const handleCreateClose = () => {
    setCreateDialogOpen(false);
  };

  const handleCreate = () => {
    const token = sessionStorage["jwt"];

    axios
      .post(`https://localhost:7273/api/School/Create`, newSchool, {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      })
      .then(() => {
        getAllSchools();
        handleCreateClose();
      })
      .catch((error) => console.error(error));
  };

  const handleCardClick = (schoolId: any) => {
    navigation(`/school-details/${schoolId}`);
  };

  const stopPropagation = (event: React.MouseEvent) => {
    event.stopPropagation();
  };
  

  return (
    <Container style={{ padding: "4em 6em" }}>
      <Box display="flex" justifyContent="flex-end">
        <Button onClick={handleCreateOpen} variant="outlined" style={{ marginRight: "-80px" }}>
          + NEW
        </Button>
      </Box>
      <Dialog open={createDialogOpen} onClose={handleCreateClose} aria-labelledby="create-dialog-title">
        <DialogTitle id="create-dialog-title">Create School</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            id="new-name"
            label="School Name"
            type="text"
            fullWidth
            value={newSchool.name}
            onChange={(e) => setNewSchool({ ...newSchool, name: e.target.value })}
          />
          <TextField
            margin="dense"
            id="new-address"
            label="Address"
            type="text"
            fullWidth
            value={newSchool.address}
            onChange={(e) => setNewSchool({ ...newSchool, address: e.target.value })}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCreateClose} color="primary">
            Cancel
          </Button>
          <Button onClick={handleCreate} color="primary">
            Create
          </Button>
        </DialogActions>
      </Dialog>
      <Grid container spacing={3} columns={{ xs: 12, sm: 8, md: 12 }} marginLeft={10} marginTop={1}>
        {schools.map((school, index) => (
          <Grid item xs={12} sm={6} md={4} key={index}>
            <Card elevation={18} className={classes.card}>
              <Box p={1.5}>
                <Box display="flex" justifyContent="space-between" alignItems="center">
                  <Link to={`/school-details/${school["id"]}`} className={classes.link}>
                    <Typography variant="h5">
                      <LocationCityIcon color="primary" />
                      {school["name"]}
                    </Typography>
                  </Link>
                  <Box>
                    <IconButton color="default" onClick={(event) => {onDelete(school["name"]); stopPropagation(event)}}>
                      <DeleteIcon />
                    </IconButton>
                    <IconButton color="default" onClick={(event) => {handleClickOpen(school); stopPropagation(event)}}>
                      <EditIcon />
                    </IconButton>
                    <Dialog open={open} onClose={(event) => handleClose(event)} aria-labelledby="form-dialog-title">
                      <DialogTitle id="form-dialog-title">Edit School</DialogTitle>
                      <DialogContent>
                        <TextField
                          autoFocus
                          margin="dense"
                          id="name"
                          label="School Name"
                          type="text"
                          fullWidth
                          value={selectedSchool["name"]}
                          onChange={(e) => setSelectedSchool({ ...selectedSchool, name: e.target.value })}
                        />
                        <TextField
                          margin="dense"
                          id="address"
                          label="Address"
                          type="text"
                          fullWidth
                          value={selectedSchool.address}
                          onChange={(e) => setSelectedSchool({ ...selectedSchool, address: e.target.value })}
                        />
                      </DialogContent>
                      <DialogActions>
                        <Button onClick={handleClose} color="primary">
                          Cancel
                        </Button>
                        <Button onClick={handleEdit} color="primary">
                          Edit
                        </Button>
                      </DialogActions>
                    </Dialog>
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
                <Button onClick={() => handleCardClick(school["id"])}>Details</Button>
              </CardActions>
            </Card>
          </Grid>
        ))}
      </Grid>
    </Container>
  );
}
