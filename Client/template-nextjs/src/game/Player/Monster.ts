import { Scene } from "phaser";

export class Monster implements IEntity
{
    id : string
    posX : number
    posY : number
    scene: Scene
    destructible : Boolean
    sprite : Phaser.GameObjects.Sprite
    Polly: Phaser.Geom.Polygon

    constructor(id : string ,x : number, y : number, scene: Scene) 
    {
        this.id = id;
        this.posX = x;
        this.posY = y;
        this.scene = scene;
        //this.sprite = scene.add.sprite(x + 8, y + 8, "monster").setScale(1)
        this.spriteName = "monster";
        this.render3d = true;
        
    }

    spriteName: string;

    destroyed: boolean;

    render3d: boolean;
    
    public Move(x:number, y:number)
    {
        this.posX = x;
        this.posY = y;
        this.sprite.x = x;
        this.sprite.y = y;
        //this.sprite.play({ key: "monster_walk", repeat: 1 }, true);
    }
}