import { Scene } from "phaser";

export class MapGenerator {
    scene: Scene
    map: number[][]
    mapDic: { [id: integer] : {sheetName: string, x : integer}}

    constructor(scene: Scene) {
        this.scene = scene;
        this.mapDic = {};
        this.mapDic[0] = { sheetName: "otsp_tiles_01", x: 144};
        this.mapDic[1] = { sheetName: "solidWall", x: -1};
        this.mapDic[2] = { sheetName: "otsp_town_01", x: 11};
        this.mapDic[3] = { sheetName: 'otsp_town_01', x: 7};
        this.mapDic[4] = { sheetName: "destructiveWall", x: -1};
    }

    GenerateMap(map: number[][]) {
        let X = 50;
        let Y = 50;
        map.map(y => {
            y.map(x => {
                if(this.mapDic[x].x !== -1)
                    this.scene.add.image(X, Y, this.mapDic[x].sheetName, this.mapDic[x].x).setScale(1.5);
                else
                    this.scene.add.image(X, Y, this.mapDic[x].sheetName).setScale(1.5);

                X += 50;
            })
            Y += 50;
            X = 50;
        })
    }
}