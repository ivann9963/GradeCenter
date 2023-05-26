import axios from "axios";
import { UserRoles } from "./models/aspNetUser";
import Discipline from "./models/discipline";

const api = axios.create({
  baseURL: 'https://localhost:7273/api',
  headers: {
    'Content-Type': 'application/json',
    Authorization: `Bearer ${sessionStorage['jwt']}`,
  },
});

api.interceptors.request.use(
  (config) => {
    if (!sessionStorage['jwt']) {
      window.location.href = '/login';
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

const getSchoolById = (schoolId: string) => api.get(`/School/GetSchoolById?schoolId=${schoolId}`);

const getAllSchools = () => api.get(`/School/GetAllSchools`);

const getAllUsers = () => api.get(`/Account/GetAllUsers`);

const getAllSchoolsClassess = () => api.get(`/SchoolClass/GetAllClassess`);

const updateUser = (userId: string, newPassword: string | undefined, newRole: UserRoles | undefined, isActive: boolean | undefined, newPhoneNumber: string | undefined) =>
  api.put(`/Account/Update?userId=${userId}&newPassword=${newPassword}&newRole=${newRole}&isActive=${isActive}&newPhoneNumber=${newPhoneNumber}`);

const addChild = (parentId: string, firstName: string, lastName: string) =>
  api.put(`/Account/AddChild?parentId=${parentId}&childFirstName=${firstName}&childLastName=${lastName}`);

const changeSchool = (newSchool: string, userId: string) =>
  api.put(`/School/Update`, {
    name: newSchool,
    users: [{
      userId: userId,
      role: 4
    }]
  });

const enroll = (userId: string, schoolClassName: string) =>
  api.put(`/SchoolClass/EnrollForClass`, {
    studentId: userId,
    SchoolClassName: schoolClassName,
  });

const withdraw = (userId: string) =>
  api.put(`/SchoolClass/WithdrawFromClass?studentId=${userId}`);

const createSchoolClass = (year: number, department: string, schoolName: string, teacherNames: string) =>
  api.post(`/SchoolClass/CreateClass`, {
    year: year,
    department: department,
    schoolName: schoolName,
    teacherNames: teacherNames,
  });

  const createCurricullum = (disciplines: Discipline[]) => 
  api.post(`/Curriculum/Create`, disciplines);


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
    createSchoolClass,
    createCurricullum
};

export default requests;
