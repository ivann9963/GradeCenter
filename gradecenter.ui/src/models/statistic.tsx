export default class Statistic {
    id: string; // This will be either school, class or teacher id
    name: string; // School, Class or Teacher name
    disciplineName: string;
    averageRate: number;
    comparedToLastWeek: number;
    comparedToLastMonth: number;
    comparedToLastYear: number;

    constructor(
        id: string,
        name: string,
        disciplineName: string,
        averageRate: number,
        comparedToLastWeek: number,
        comparedToLastMonth: number,
        comparedToLastYear: number
    ) {
        this.id = id;
        this.name = name;
        this.averageRate = averageRate;
        this.comparedToLastWeek = comparedToLastWeek;
        this.comparedToLastMonth = comparedToLastMonth;
        this.comparedToLastYear = comparedToLastYear;
        this.disciplineName = disciplineName;
    }
}