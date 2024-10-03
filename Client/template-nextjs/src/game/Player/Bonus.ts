import { Scene } from "phaser";

export class Bonus
{
    id : string
    posX : number
    posY : number
    scene: Scene
    destructible : Boolean
    sprite : Phaser.GameObjects.Image
    bonusType : number

    constructor(id : string ,x : number, y : number, bonusType : number, scene: Scene) 
    {
        console.log("CREATE BONUS",x, y)
        this.id = id;
        this.posX = x;
        this.posY = y;
        this.scene = scene;
        this.bonusType = bonusType;

        if(bonusType === 1)
            this.sprite = scene.add.image(x + 8, y + 8, "bomb").setScale(0.8)
        if(bonusType === 2)
            this.sprite = scene.add.image(x + 8, y + 8, "dead").setScale(0.8)
        if(bonusType === 3)
            this.sprite = scene.add.image(x + 8, y + 8, "logo").setScale(0.8)
        
    }
}