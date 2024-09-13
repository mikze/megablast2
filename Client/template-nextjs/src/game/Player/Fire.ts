import { Scene } from "phaser";

export class Fire
{
    posX : number
    posY : number
    image: Phaser.GameObjects.Image

    constructor(posX : number, posY : number) 
    {
        this.posX = posX;
        this.posY = posY;
    }
}