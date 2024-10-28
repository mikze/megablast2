import { Scene } from "phaser";

export class Monster
{
    id : string
    posX : number
    posY : number
    scene: Scene
    destructible : Boolean
    sprite : Phaser.GameObjects.Image

    constructor(id : string ,x : number, y : number, scene: Scene) 
    {
        console.log("CREATE MONSTER",x, y)
        this.id = id;
        this.posX = x;
        this.posY = y;
        this.scene = scene;

        this.sprite = scene.add.image(x + 8, y + 8, "Monster").setScale(0.2)     
    }
}