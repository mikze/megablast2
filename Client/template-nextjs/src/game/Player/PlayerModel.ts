export class PlayerModel {
    
    id : string
    name : string
    posX : number
    posY : number
    x : number
    y : number
    dead : boolean

    constructor(id : string, name : string, x : number, y : number) {
        this.id = id;
        this.posX = x;
        this.posY = y;
        this.name = name;
      }
}