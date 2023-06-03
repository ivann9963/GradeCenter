import { AspNetUser } from "./aspNetUser";
import Discipline from "./discipline";

export class Grade {
    rate: number | null;
    student: AspNetUser | null;
    discipline: Discipline | null;
  
    constructor(
      rate: number | null = null,
      student: AspNetUser | null = null,
      discipline: Discipline | null = null
    ) {
      this.rate = rate;
      this.student = student;
      this.discipline = discipline;
    }
  }
  