type AspNetUser = {
    // Define the properties of AspNetUser here
  };
  
  type SchoolClass = {
    // Define the properties of SchoolClass here
  };
  
export interface School {
    id: string;
    name: string;
    address: string;
    isActive: boolean;
    // people: AspNetUser[];
    // schoolClasses: SchoolClass[];
  }
  
  