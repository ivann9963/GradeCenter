import { AspNetUser } from "./aspNetUser";
import { School } from "./school";

interface Discipline {
  // Define the properties of Discipline here
}

export class SchoolClass {
  id: string;
  year: number;
  teacherId: number;
  department: string;
  headTeacher: AspNetUser;
  school: School;
  schoolName: string | undefined;
  students: AspNetUser[];
  curriculum: Discipline[];

  constructor(
    id: string = "",
    year: number = 0,
    department: string = "",
    teacherId: number = 0,
    headTeacher: AspNetUser = new AspNetUser(),
    school: School,
    schoolName: string,
    students: AspNetUser[] = [],
    curriculum: Discipline[] = []
  ) {
    this.id = id;
    this.year = year;
    this.teacherId = teacherId;
    this.department = department;
    this.headTeacher = headTeacher;
    this.school = school;
    this.schoolName = schoolName;
    this.students = students;
    this.curriculum = curriculum;
  }
}
