import { Scene } from "phaser"
import { Bomb } from "./Bomb"

  
  export class Player {


    id : string
    name : string
    posX : number
    posY : number
    x : number
    y : number
    textName : Phaser.GameObjects.Text
    sprite : Phaser.GameObjects.Sprite
    scene: Scene
    dead : boolean
    skinName : string
    constructor(id : string, name : string, x : number, y : number, scene: Scene, skinName: string) {
      this.id = id;
      this.textName = scene.add.text(x, y + 20, name, {
        fontFamily: 'Arial Black', fontSize: 18, color: '#ffffff',
        stroke: '#000000', strokeThickness: 8,
        align: 'center'
      }).setOrigin(0.5).setDepth(100);
      this.posX = x;
      this.posY = y;
      this.sprite = scene.add.sprite(50, 300, skinName).setScale(1);
      this.sprite.x = x;
      this.sprite.y = y;
      this.textName.x = x;
      this.textName.y = y - 20;
      this.scene = scene;
      this.name = name;
      this.skinName = skinName
    }

    Destroy()
    {
      this.sprite.destroy();
      this.textName.destroy();
    }

    Say(message : string)
    {
      let msg = this.scene.add.text(this.posX, this.posY + 20, this.name+": "+message, {
        fontFamily: 'Arial Black', fontSize: 11, color: '#71e023',
        stroke: '#000000', strokeThickness: 7,
        align: 'center'
      }).setOrigin(0.5).setDepth(100);

      setTimeout(() => msg.destroy(), 2000);
    }

    Move(x: number, y: number)
    {
      if (this.posX > x)
        this.sprite.play({ key: this.skinName + "_walkLeft", repeat: 1 }, true);
      if (this.posX < x)
        this.sprite.play({ key: this.skinName + "_walkRight", repeat: 1 }, true);
      if (this.posY < y)
        this.sprite.play({ key: this.skinName + "_walkDown", repeat: 1 }, true);
      if (this.posY > y)
        this.sprite.play({ key: this.skinName + "_walkUp", repeat: 1 }, true);

      this.posX = x;
      this.posY = y;
      this.x = x;
      this.y = y;
      this.sprite.x = x;
      this.sprite.y = y; 
      this.textName.x = x;
      this.textName.y = y - 20;
    }

    Dead()
    {
      this.dead = true;
      this.sprite.destroy();
      this.sprite = this.scene.add.sprite(this.posX, this.posY, "dead").setScale(0.8)     
      this.scene.sound.play("kill", {volume : 0.1});
    }

    Alive()
    {
      this.dead = false;
      this.sprite.destroy();
      this.sprite = this.scene.add.sprite(50, 300, this.skinName).setScale(1);
      this.textName.destroy();
      this.textName = this.scene.add.text(this.x, this.y + 20, this.name, {
        fontFamily: 'Arial Black', fontSize: 18, color: '#ffffff',
        stroke: '#000000', strokeThickness: 8,
        align: 'center'
      }).setOrigin(0.5).setDepth(100);
    }
  }