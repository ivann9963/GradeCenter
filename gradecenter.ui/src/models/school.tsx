import { AspNetUser } from "./aspNetUser";
import { SchoolClass } from "./schoolClass";

  
export interface School {
    id: string;
    name: string;
    address: string;
    isActive: boolean;
    people: AspNetUser[];
    schoolClasses: SchoolClass[];
  }
  
  