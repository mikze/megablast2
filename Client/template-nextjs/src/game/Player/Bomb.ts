import { Scene } from "phaser";

export class Bomb implements  IEntity{
  id: string
  posX: number
  posY: number
  scene: Scene
  destructible: Boolean
  sprite: Phaser.GameObjects.Image
    Polly: Phaser.Geom.Polygon
    render3d: boolean;
  constructor(x: number, y: number, scene: Scene, id: string) {
    this.id = id;
    this.posX = x;
    this.posY = y;
    this.scene = scene;
    this.destructible = true;
    //this.sprite = this.scene.add.image(this.posX - 8, this.posY - 8, "bomb").setScale(0.8);
    this,this.spriteName = "bomb";
    this.destructible = true;
    this.render3d = true;
  }

    spriteName: string;

    destroyed: boolean;

    
}