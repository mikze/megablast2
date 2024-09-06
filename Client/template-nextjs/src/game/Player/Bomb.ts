import { Scene } from "phaser";

export class Bomb
{
    x : number
    y : number
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

      setTimeout(() => 
        {
            sprite.destroy();
            let sprites = [
                this.scene.add.image(sprite.x + 32, sprite.y, "fire").setScale(0.8),
                this.scene.add.image(sprite.x + 64, sprite.y, "fire").setScale(0.8),
                this.scene.add.image(sprite.x + 96, sprite.y, "fire").setScale(0.8),

                this.scene.add.image(sprite.x - 32, sprite.y, "fire").setScale(0.8),
                this.scene.add.image(sprite.x - 64, sprite.y, "fire").setScale(0.8),
                this.scene.add.image(sprite.x - 96, sprite.y, "fire").setScale(0.8),

                this.scene.add.image(sprite.x, sprite.y + 32, "fire").setScale(0.8),
                this.scene.add.image(sprite.x, sprite.y + 64, "fire").setScale(0.8),
                this.scene.add.image(sprite.x, sprite.y + 96, "fire").setScale(0.8),

                this.scene.add.image(sprite.x, sprite.y - 32, "fire").setScale(0.8),
                this.scene.add.image(sprite.x, sprite.y - 64, "fire").setScale(0.8),
                this.scene.add.image(sprite.x, sprite.y - 96, "fire").setScale(0.8)
            ]

            setTimeout(() => { sprites.map( s => s.destroy()) }, 2000); }, 2000);
    }
}