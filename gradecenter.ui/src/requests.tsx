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

const getAllSchools = () => {
    const url = `https://localhost:7273/api/School/GetAllSchools`;
    const token = sessionStorage["jwt"];

    return axios({
      method: "get",
      url: url,
      headers: {
        "Content-Type": "text/plain;charset=utf-8",
        Authorization: `Bearer ${token}`,
      },
    });
};

const getAllUsers = () => {
  const url = `https://localhost:7273/api/Account/GetAllUsers`;
    const token = sessionStorage["jwt"];

    return axios({
      method: "get",
      url: url,
      headers: {
        "Content-Type": "text/plain;charset=utf-8",
        Authorization: `Bearer ${token}`,
      },
    });
}

const getAllSchoolsClassess = () => {
  const url = `https://localhost:7273/api/SchoolClass/GetAllClassess`;
  const token = sessionStorage["jwt"];

  return axios({
    method: "get",
    url: url,
    headers: {
      "Content-Type": "text/plain;charset=utf-8",
      Authorization: `Bearer ${token}`,
    },
  });
}

// Add more requests here...

const requests = {
    getSchoolById,
    getAllSchools,
    getAllUsers,
    getAllSchoolsClassess
};

export default requests;
