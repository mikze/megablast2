import { Scene } from "phaser"
import { Bomb } from "./Bomb"
import { MapGenerator } from "../scenes/MapGenerator"
import { RayCaster } from "./RayCaster"
import { GameLevel } from "../scenes/GameLevel"

  export class Player{
      id: string
      name: string
      posX: number
      posY: number
      x: number
      y: number
      textName: Phaser.GameObjects.Text
      sprite: Phaser.GameObjects.Sprite
      scene: GameLevel
      dead: boolean
      skinName: string
      caster: RayCaster
      angle: number

      constructor(id: string, name: string, x: number, y: number, scene: GameLevel, skinName: string) {

          this.id = id;
          this.textName = scene.add.text(x, y + 20, name, {
              fontFamily: 'Arial Black', fontSize: 18, color: '#ffffff',
              stroke: '#000000', strokeThickness: 8,
              align: 'center'
          }).setOrigin(0.5).setDepth(100);
          this.posX = x;
          this.posY = y;
          //this.sprite = scene.add.sprite(50, 300, skinName).setScale(1);
          //this.sprite.x = x;
          //this.sprite.y = y;
          this.textName.x = x;
          this.textName.y = y - 20;
          this.scene = scene;
          this.name = name;
          this.skinName = skinName
          this.caster = new RayCaster(scene, this.posX, this.posY,
              [new Phaser.Geom.Polygon([100, 50, 200, 80, 140, 210, 100, 150, 100, 50]),
                  new Phaser.Geom.Polygon([100, 200, 120, 250, 60, 300, 100, 200]),
                  new Phaser.Geom.Polygon([170, 270, 250, 320, 290, 440, 150, 380, 170, 270]),
                  new Phaser.Geom.Polygon([260, 40, 270, 70, 240, 60, 260, 40]),
                  new Phaser.Geom.Polygon([480, 50, 380, 150, 300, 90, 480, 50]),
                  new Phaser.Geom.Polygon([350, 190, 460, 170, 440, 270, 330, 290, 350, 190]),
                  new Phaser.Geom.Polygon([0, 0, 1000, 0, 1000, 1000, 0, 1000, 0, 0]),
                  new Phaser.Geom.Polygon([100, 50, 200, 80, 140, 210, 100, 150, 100, 50])]);

          this.caster = new RayCaster(scene, this.posX, this.posY, MapGenerator.GetPolygons());

          this.angle = 30;
      }

      Destroy() {
          this.sprite.destroy();
          this.textName.destroy();
      }

      Say(message: string) {
          let msg = this.scene.add.text(this.posX, this.posY + 20, this.name + ": " + message, {
              fontFamily: 'Arial Black', fontSize: 11, color: '#71e023',
              stroke: '#000000', strokeThickness: 7,
              align: 'center'
          }).setOrigin(0.5).setDepth(100);

          setTimeout(() => msg.destroy(), 2000);
      }

      Move(x: number, y: number) {
          // if (this.posX > x)
          //     this.sprite.play({key: this.skinName + "_walkLeft", repeat: 1}, true);
          // if (this.posX < x)
          //     this.sprite.play({key: this.skinName + "_walkRight", repeat: 1}, true);
          // if (this.posY < y)
          //     this.sprite.play({key: this.skinName + "_walkDown", repeat: 1}, true);
          // if (this.posY > y)
          //     this.sprite.play({key: this.skinName + "_walkUp", repeat: 1}, true);

          this.posX = x;
          this.posY = y;
          this.x = x;
          this.y = y;
          //this.sprite.x = x;
          //this.sprite.y = y;
          this.textName.x = x;
          this.textName.y = y - 20;

      }

      Dead() {
          this.dead = true;
          this.sprite.destroy();
          this.sprite = this.scene.add.sprite(this.posX, this.posY, "dead").setScale(0.8)
          this.scene.sound.play("kill");
      }

      Alive() {
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

      Update() {
          if (this.caster.graphics !== undefined)
              this.caster.graphics.clear();
          this.caster.setTo2(this.x, this.y, this.angle);
          this.caster.drawView(this.caster.castCone());
          //this.caster.renderObstacles();
      }
  }
  