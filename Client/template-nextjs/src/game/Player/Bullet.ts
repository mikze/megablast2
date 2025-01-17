import { Scene } from "phaser";

export class Bullet implements IEntity
{
    id : string
    posX : number
    posY : number
    scene: Scene
    destructible : Boolean
    sprite : Phaser.GameObjects.Image
    Polly: Phaser.Geom.Polygon
    render3d: boolean;
    constructor(id : string ,x : number, y : number, scene: Scene)
    {
        this.id = id;
        this.posX = x;
        this.posY = y;
        this.scene = scene;
        //this.sprite = scene.add.image(x + 8, y + 8, "fire").setScale(1)
        this.spriteName = "fire";
        this.render3d = true;
    }

    spriteName: string;

    destroyed: boolean;

    

    public Move(x:number, y:number)
    {
        this.posX = x;
        this.posY = y;
        this.sprite.x = x;
        this.sprite.y = y;
    }
}