import { School } from "./school";

interface SchoolClass {
  // Define the properties of SchoolClass here
}

interface UserRelation {
  // Define the properties of UserRelation here
}

export enum UserRoles {
  Admin,
  Principle,
  Teacher,
  Parent,
  Student,
}

export class AspNetUser {
  // IdentityUser<Guid> properties here
  id: string | null;
  firstName: string | null;
  lastName: string | null;
  schoolId: string | null;
  school: any | null;
  isActive: boolean | null;
  schoolClassId: string | null;
  userRole: UserRoles | null;
  schoolClass: SchoolClass | null;
  childrenRelations: UserRelation[];
  parentRelations: UserRelation[];
  schoolName: string | null;

  constructor(
    id: string | null = null,
    firstName: string | null = null,
    lastName: string | null = null,
    schoolId: string | null = null,
    schoolClassId: string | null = null,
    school: School | null = null,
    schoolName: string | null = null,
    isActive: boolean | null = null,
    userRole: UserRoles | null = null,
    schoolClass: SchoolClass | null = null,
    childrenRelations: UserRelation[] = [],
    parentRelations: UserRelation[] = []
  ) {
    this.id = id;
    this.firstName = firstName;
    this.lastName = lastName;
    this.schoolId = schoolId;
    this.school = school;
    this.schoolName = schoolName;
    this.isActive = isActive;
    this.schoolClassId = schoolClassId;
    this.userRole = userRole;
    this.schoolClass = schoolClass;
    this.childrenRelations = childrenRelations;
    this.parentRelations = parentRelations;
  }
}
