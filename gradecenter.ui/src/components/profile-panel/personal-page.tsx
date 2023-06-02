import { Box, Container, TextField } from "@mui/material";
import { AspNetUser, UserRoles } from "../../models/aspNetUser";
import AccountCircleIcon from '@mui/icons-material/AccountCircle';
import Typography from "@material-ui/core/Typography/Typography";

interface Profile {
    profile: AspNetUser | null;
}

export default function Personal(params: Profile){
    const {firstName, lastName, userRole} = params.profile as AspNetUser;

    // TODO: Add More Details
    
    return(
        <Container>
        <Typography variant="h4" align="center" style={{marginBottom:70}}>Credentials</Typography>
        <Box style={{ display:"flex" ,marginBottom:"50px", justifyContent:"center"}}>
        <TextField
             style={{width: "55%"}}
             id="firstName" 
             variant="outlined"
             label = "First Name"
             value= {firstName}
             InputProps={{
                readOnly: true,
             }}
        />
        </Box>
        <Box style={{ display:"flex" , marginBottom:"50px", justifyContent:"center"}}>
        <TextField 
             style={{width: "55%"}}
             id="lastName" 
             variant="outlined"
             label = "Last Name"
             value={lastName}
             InputProps={{
                readOnly: true,
             }}
        />
        </Box>
        <Box style={{display:"flex", justifyContent:"center"}}>
        <TextField 
             style={{width: "55%"}}
             id="userRole" 
             label="User Role"
             value={UserRoles[userRole as number]}
             variant="outlined"
             InputProps={{
                readOnly: true,
             }}
        />
        </Box>
        </Container>
    )
}