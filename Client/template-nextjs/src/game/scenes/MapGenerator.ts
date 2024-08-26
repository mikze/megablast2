import { Scene } from "phaser";
import { Connection } from "../SignalR/Connection";

export class MapGenerator {
    scene: Scene
    map: number[][]

    constructor(scene: Scene) {
        this.scene = scene;
    }

    GenerateMap(map: number[][]) {
        console.log(map);
        let X = 50;
        let Y = 50;
        map.map(y => {
            y.map(x => {
                if (x !== 0) {
                    this.scene.add.sprite(X, Y, "solidWall").setScale(1);
                }
                X += 50;
            })
            Y += 50;
            X = 50;
        })
    }
}