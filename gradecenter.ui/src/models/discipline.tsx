class Discipline {
    name: string;
    schoolClassId: string;
    teacherId: string;
    occuranceTime?: string | undefined;
    occuranceDay?: number | undefined;

    constructor(
        name: string,
        schoolClassId: string,
        teacherId: string,
        occuranceTime?: string | undefined,
        occuranceDay?: number | undefined
    ) {
        this.name = name;
        this.schoolClassId = schoolClassId;
        this.teacherId = teacherId;
        this.occuranceTime = occuranceTime;
        this.occuranceDay = occuranceDay;

    }
}

export default Discipline;
