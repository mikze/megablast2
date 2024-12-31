import { Scene } from "phaser"
import { Bomb } from "./Bomb"
import { MapGenerator } from "../scenes/MapGenerator"


class RayCaster extends Phaser.Geom.Triangle{

    x : number
    y : number
    angle : number
    fov : number
    scene: Scene
    ray : Phaser.Geom.Line
    debug : boolean
    objects: Phaser.Geom.Polygon[]
    graphics: Phaser.GameObjects.Graphics
    heading : Phaser.Math.Vector2
    images: Phaser.GameObjects.Image[]
    
    constructor(scene: Scene, x: number, y: number, objects: Phaser.Geom.Polygon[]) {
        super(length, 0, -length, -length, -length, length);
        this.scene = scene;
        this.x = x;
        this.y = y;
        this.angle = Math.PI / 4;
        this.fov = Math.PI / 4; // set default to 45 degrees
        this.ray = new Phaser.Geom.Line(this.x, this.y, this.x + 1, this.y); // line of unit length
        this.objects = objects;
        this.debug = true;
        this.graphics = this.scene.add.graphics();
        this.heading = new Phaser.Math.Vector2(1,0); // unit vector facing heading
        this.images = [];
    }

    setCone(fov = Math.PI / 4) {
        this.fov = fov;
    }

    setTo2(x: number,y:number, angle:number) {
        this.x = x;
        this.y = y;
        this.angle = angle;
        this.heading.setAngle(angle);
    }

    cast() {
        return this.castRay(this.angle)
    }

    castCone() {
        let view = [];
        for (let i = this.angle-this.fov/2; i < this.angle + this.fov/2; i += this.fov / 150 ) // rays def 30
        {
            view.push(this.castRay(i))
        };
        return view
    }

    castRay(angle : number) {
        Phaser.Geom.Line.SetToAngle(this.ray, this.x, this.y, angle, 1);
        const cp = Phaser.Geom.Intersects.GetLineToPolygon(this.ray, this.objects, true);
        if (this.debug && cp !== null) {
            this.graphics.lineStyle(1,0x00ff00).lineBetween(this.x, this.y, cp.x, cp.y);
        }
        
        return cp;
    }

    renderObstacles() {
        this.objects.forEach(s => {
            this.graphics.lineStyle(1, 0x00ff00).strokePoints(s.points, true)
        });
    }

    draw() {
        Phaser.Geom.Triangle.CenterOn(this, this.x, this.y);
        this.graphics.lineStyle(1,0x00ff00).strokeTriangleShape(this);
    }

    drawView(view: Phaser.Math.Vector4[]) {
        const sliceWidth = 500/view.length;
        this.images.forEach(i => i.destroy());
        this.images = [];
        const image1 = this.scene.add.image(50 * sliceWidth,(500) / 2, 'destructiveWall');
        // @ts-ignore
        image1.posX =650;
        // @ts-ignore
        image1.posY = 200;
        view.forEach((vec, i) => {
            if (vec !== null) {
                const ray = new Phaser.Math.Vector2(vec.x - this.x, vec.y - this.y);
                const d = vec.z;
                const h = 550 / ray.dot(this.heading); // 25,000     
                const b = (Math.max(900 - d, 0)) / 400; // 400

                
                const image = this.scene.add.image(100 +i * sliceWidth,(100-h) / 2, 'dude');//destructiveWall
                image.scaleY = h;
                image.scaleX = sliceWidth;

                // @ts-ignore
                const xLen = (image1.posX - this.x)/100;
                // @ts-ignore
                const yLen = (image1.posY - this.y)/100;
                const c = Math.sqrt( Math.pow(yLen, 2) + Math.pow(xLen, 2));
                image1.scaleY = 2/c;
                image1.scaleX = 2/c;
                image1.setDepth(1);
                image.setDepth(1/b);
                this.images.push(image);
                this.images.push(image1);
            }
        })
    }
}

  export class Player{


      id: string
      name: string
      posX: number
      posY: number
      x: number
      y: number
      textName: Phaser.GameObjects.Text
      sprite: Phaser.GameObjects.Sprite
      scene: Scene
      dead: boolean
      skinName: string
      caster: RayCaster
      angle: number

      constructor(id: string, name: string, x: number, y: number, scene: Scene, skinName: string) {

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
          if (this.posX > x)
              this.sprite.play({key: this.skinName + "_walkLeft", repeat: 1}, true);
          if (this.posX < x)
              this.sprite.play({key: this.skinName + "_walkRight", repeat: 1}, true);
          if (this.posY < y)
              this.sprite.play({key: this.skinName + "_walkDown", repeat: 1}, true);
          if (this.posY > y)
              this.sprite.play({key: this.skinName + "_walkUp", repeat: 1}, true);

          this.posX = x;
          this.posY = y;
          this.x = x;
          this.y = y;
          this.sprite.x = x;
          this.sprite.y = y;
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
          this.caster.renderObstacles();
      }
  }
  