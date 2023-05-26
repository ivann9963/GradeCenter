class Discipline {
    Name: string;
    SchoolClassId: string;
    TeacherId: string;

    constructor(
        name: string,
        schoolClassId: string,
        teacherId: string,
    ) {
        this.Name = name;
        this.SchoolClassId = schoolClassId;
        this.TeacherId = teacherId;
    }
}

export default Discipline;
