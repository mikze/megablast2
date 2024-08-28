import { Scene } from "phaser"

  
  export class Player {

    id : string
    name : string
    x : number
    y : number
    textName : Phaser.GameObjects.Text
    sprite : Phaser.GameObjects.Sprite
    scene: Scene
  
    constructor(id : string, name : string, x : number, y : number, scene: Scene) {
      this.id = id;
      this.textName = scene.add.text(x, y + 20, name, {
        fontFamily: 'Arial Black', fontSize: 38, color: '#ffffff',
        stroke: '#000000', strokeThickness: 8,
        align: 'center'
      }).setOrigin(0.5).setDepth(100);
      this.x = x;
      this.y = y;
      this.sprite = scene.add.sprite(50, 300, "playerSprite").setScale(1);
      this.sprite.x = x;
      this.sprite.y = y;
      this.textName.x = x;
      this.textName.y = y - 20;
      this.scene = scene;
    }

    Say(message : string)
    {
      let msg = this.scene.add.text(this.x, this.y + 20, message, {
        fontFamily: 'Arial Black', fontSize: 38, color: '#ffffff',
        stroke: '#000000', strokeThickness: 8,
        align: 'center'
      }).setOrigin(0.5).setDepth(100);

      setTimeout(() => msg.destroy(), 2000);
    }

    Move(x: number, y: number)
    {
      if (this.x > x)
        this.sprite.play({ key: "walkLeft", repeat: 1 }, true);
      if (this.x < x)
        this.sprite.play({ key: "walkRight", repeat: 1 }, true);
      if (this.y < y)
        this.sprite.play({ key: "walkDown", repeat: 1 }, true);
      if (this.y > y)
        this.sprite.play({ key: "walkUp", repeat: 1 }, true);

      this.x = x;
      this.y = y;
      this.sprite.x = x;
      this.sprite.y = y; 
      this.textName.x = x;
      this.textName.y = y - 20;
    }
  }
