// requests.tsx
import axios from "axios";
import { UserRoles } from "./models/aspNetUser";

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

const updateUser = (userId: string, newPassword: string | undefined, newRole: UserRoles | undefined, isActive: boolean | undefined, newPhoneNumber: string | undefined) => {
  const url = `https://localhost:7273/api/Account/Update`;
  const token = sessionStorage["jwt"];

  return axios({
    method: "put",
    url: url,
    params: {
      userId: userId,
      newPassword: newPassword,
      newRole: newRole,
      isActive: isActive,
      newPhoneNumber: newPhoneNumber 
    },
    headers: {
      "Content-Type": "text/plain;charset=utf-8",
      Authorization: `Bearer ${token}`,
    },
  });
}

const addChild = (parentId: string, firstName: string, lastName: string) => {
  const url = `https://localhost:7273/api/Account/AddChild`;
  const token = sessionStorage["jwt"];

  return axios({
    method: "put",
    url: url,
    params: {
      parentId: parentId,
      childFirstName: firstName,
      childLastName: lastName
    },
    headers: {
      "Content-Type": "text/plain;charset=utf-8",
      Authorization: `Bearer ${token}`,
    },
  });
}

const changeSchool = (newSchool: string, userId: string) => {
  const url = `https://localhost:7273/api/School/Update`;
  const token = sessionStorage["jwt"];

  return axios({
    method: "put",
    url: url,
    data: {
        name: newSchool,
        users: [
          {
            userId: userId,
            role: 4
          }
        ]
      },
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
  });
}

const enroll = (userId: string, schoolClassName: string) => {
  const url = `https://localhost:7273/api/SchoolClass/EnrollForClass`;
  const token = sessionStorage["jwt"];

  return axios({
    method: "put",
    url: url,
    data: {
      studentId: userId,
      SchoolClassName: schoolClassName,
      },
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
  });
}

const withdraw = (userId: string) => {
  const url = `https://localhost:7273/api/SchoolClass/WithdrawFromClass/?studentId=${userId}`;
  const token = sessionStorage["jwt"];

  return axios({
    method: "put",
    url: url,
    data: {
      studentId: userId,
      },
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
  });
}

const createSchoolClass = (year: number, department: string, schoolName: string, teacherNames: string) => {
  const url = `https://localhost:7273/api/SchoolClass/CreateClass`;
  const token = sessionStorage["jwt"];

  return axios({
    method: "post",
    url: url,
    data: {
       year: year,
       department: department,
       schoolName: schoolName,
       teacherNames: teacherNames,
      },
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
  });
}

const requests = {
    getSchoolById,
    getAllSchools,
    getAllUsers,
    getAllSchoolsClassess,
    updateUser,
    addChild,
    changeSchool,
    enroll,
    withdraw,
    createSchoolClass
};

export default requests;
