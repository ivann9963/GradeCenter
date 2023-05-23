// requests.tsx
import axios from "axios";

const getSchoolById = (schoolId: string) => {
    const url = `https://localhost:7273/api/School/GetSchoolById`;
    const token = sessionStorage["jwt"];

    return axios({
      method: "get",
      url: url,
      params: {
        schoolId: schoolId,
      },
      headers: {
        "Content-Type": "text/plain;charset=utf-8",
        Authorization: `Bearer ${token}`,
      },
    });
};

// Add more requests here...

const requests = {
    getSchoolById,
    // Add more requests here...
};

export default requests;
