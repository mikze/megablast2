export class BonusModel
{
    id : string
    posX : number
    posY : number
    bonusType : number

    constructor(posX : number, posY : number) 
    {
        this.posX = posX;
        this.posY = posY;
    }
}