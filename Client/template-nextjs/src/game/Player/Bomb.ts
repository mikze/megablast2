import { Scene } from "phaser";

export class Bomb
{
    posX : number
    posY : number
    scene: Scene

    constructor(x : number, y : number, scene: Scene) 
    {
        this.x = x;
        this.y = y;
        this.scene = scene;
    }

    PlantBomb()
    {
      let sprite = this.scene.add.image(this.x + 8, this.y + 8, "bomb").setScale(0.8);

      setTimeout(() => { this.scene.sound.play("boom"); sprite.destroy(); } , 2000);
    }
}