import { AspNetUser } from "./aspNetUser";

export class UserRelation{
    parent: AspNetUser | null
    child: AspNetUser | null
    constructor(parent: AspNetUser| null, child: AspNetUser | null){
        this.parent = parent;
        this.child = child;
    }
}