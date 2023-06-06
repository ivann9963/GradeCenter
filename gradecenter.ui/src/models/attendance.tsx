import { AspNetUser } from "./aspNetUser";
import Discipline from "./discipline";

export class Attendance{
    id: string | null;
    date: Date | null;
    discipline: Discipline | null;
    student: AspNetUser | null;
    hasAttended: boolean | null;

    constructor(
        id: string | null, 
        date: Date | null, 
        discipline: Discipline | null,
        student: AspNetUser | null, 
        hasAttended: boolean | null
    ){
        this.id = id;
        this.date = date;
        this.discipline = discipline;
        this.student = student;
        this.hasAttended = hasAttended;
    }
}