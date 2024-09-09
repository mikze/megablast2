export class Wall
{
    id : string
    posX : number
    posY : number
    destructible : Boolean
    sprite : Phaser.GameObjects.Image

    constructor(x : number, y : number, Destructible:  Boolean) 
    {
        this.posX = x;
        this.posY = y;
        this.destructible = Destructible;
    }
}