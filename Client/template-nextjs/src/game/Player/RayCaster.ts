import { Scene } from "phaser"
import { GameLevel } from "../scenes/GameLevel"
import { Wall } from "./Wall"

export class RayCaster extends Phaser.Geom.Triangle{

    numberOfRays : number
    x : number
    y : number
    angle : number
    fov : number
    scene: GameLevel
    ray : Phaser.Geom.Line
    debug : boolean
    objects: Phaser.Geom.Polygon[]
    graphics: Phaser.GameObjects.Graphics
    heading : Phaser.Math.Vector2
    images: Phaser.GameObjects.Image[]

    constructor(scene: GameLevel, x: number, y: number, objects: Phaser.Geom.Polygon[]) {
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
        this.numberOfRays = 150;
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
        for (let i = this.angle-this.fov/2; i < this.angle + this.fov/2; i += this.fov / this.numberOfRays ) //
        {
            view.push(this.castRay(i))
        };
        return view
    }

    castRay(angle : number) {
        Phaser.Geom.Line.SetToAngle(this.ray, this.x, this.y, angle, 1);
        const cp = Phaser.Geom.Intersects.GetLineToPolygon(this.ray, this.objects, true);
        // if (this.debug && cp !== null) {
        //     this.graphics.lineStyle(1,0x00ff00).lineBetween(this.x, this.y, cp.x, cp.y);
        // }

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

    drawblock()
    {
        
        this.scene.entities.map(m => {   
            if(m.render3d && !m.destroyed) {
                let rad = this.heading.angle();
                const A = Math.abs(this.y - m.posY);
                const B = Math.abs(this.x - m.posX);
                const C = Math.sqrt(A * A + B * B);
                let kat = Math.asin(A / C);

                if (this.x > m.posX && this.y < m.posY)
                    kat = Math.PI - kat;
                if (this.x > m.posX && this.y > m.posY)
                    kat = Math.PI + kat;
                if (this.x < m.posX && this.y > m.posY)
                    kat = 2 * Math.PI - kat;

                
                const stopnie = kat * 180 / Math.PI;
                const an = rad * 180 / Math.PI;
                let anglediff = (an - stopnie + 180 + 360) % 360 - 180

                if (anglediff <= 22.5 && anglediff >= -22.5) {
                    const x = 500 / 45;
                    m.sprite = this.scene.add.sprite(350 + anglediff * -x, 50, m.spriteName
                    );
                    const xLen = (m.posX - this.x) / 100;
                    const yLen = (m.posY - this.y) / 100;
                    const c = Math.sqrt(Math.pow(yLen, 2) + Math.pow(xLen, 2));

                    m.sprite.scaleY = 5 / c;
                    m.sprite.scaleX = 5 / c;
                    m.sprite.setDepth(10);
                    this.images.push(m.sprite);
                }
            }
        })

        this.scene.players.map(m => {
            let rad = this.heading.angle();
            const A = Math.abs(this.y - m.posY);
            const B = Math.abs(this.x - m.posX);
            const C = Math.sqrt(A * A + B * B);
            let kat = Math.asin(A / C);

            if (this.x > m.posX && this.y < m.posY)
                kat = Math.PI - kat;
            if(this.x > m.posX && this.y > m.posY)
                kat = Math.PI  + kat;
            if(this.x < m.posX && this.y > m.posY)
                kat = 2 * Math.PI - kat;


            let image1 = null;
            const stopnie = kat*180/Math.PI;
            const  an =rad*180/Math.PI;
            let anglediff = (an - stopnie + 180 + 360) % 360 - 180

            if (anglediff <= 22.5 && anglediff>=-22.5)
            {
                const x = 1000/45;
                //console.log(m.sprite.texture.key)
                image1 = this.scene.add.image(1250+anglediff*-x , 50, m.sprite.texture.key);
                const xLen = (m.posX - this.x)/100;
                const yLen = (m.posY- this.y)/100;
                const c = Math.sqrt( Math.pow(yLen, 2) + Math.pow(xLen, 2));

                image1.scaleY = 5 / c;
                image1.scaleX = 5 / c;
                image1.setDepth(10);
                this.images.push(image1);
            }
        })
    }
    drawView(view: Phaser.Math.Vector4[]) {
        console.log(this.scene.entities.length, this.images.length);
        const sliceWidth = 1000 / view.length;
        this.images.forEach(i => i.destroy());
        this.images = [];

        this.drawblock();
        view.forEach((vec, i) => {
            if (vec !== null) {
                const ray = new Phaser.Math.Vector2(vec.x - this.x, vec.y - this.y);
                const d = vec.z;
                const h = 550 / ray.dot(this.heading); // 25,000     
                const b = (Math.max(900 - d, 0)) / 400; // 400


                const image = this.scene.add.image(100 +i * sliceWidth,(100-h) / 2, 'dude');
                image.scaleY = h;
                image.scaleX = sliceWidth;
                image.setDepth(1/b);
                this.images.push(image);

            }
        })
    }
}