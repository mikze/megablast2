import { Wall } from "../Player/Wall";
import { GameLevel } from "./Chat";

export class MapGenerator {
    map: Wall[]
    mapDic: { [id: integer] : {sheetName: string, x : integer}}
    gameLevel: GameLevel

    constructor(gameLevel: GameLevel) {
        this.mapDic = {};
        this.mapDic[0] = { sheetName: "otsp_tiles_01", x: 144};
        this.mapDic[1] = { sheetName: "solidWall", x: -1};
        this.mapDic[2] = { sheetName: "otsp_town_01", x: 11};
        this.mapDic[3] = { sheetName: 'otsp_town_01', x: 7};
        this.mapDic[4] = { sheetName: "destructiveWall", x: -1};
        this.gameLevel = gameLevel;
    }

    GenerateMap(map: Wall[]) {
        this.gameLevel.entities.map(
            e => {
                e.sprite.destroy();
            }
        );

        this.gameLevel.entities = new Array<IEntity>();
        
        map.map(wall => {
                if(!wall.destructible)
                    wall.sprite = this.gameLevel.add.image(wall.posX, wall.posY, this.mapDic[1].sheetName, this.mapDic[1].x).setScale(1);
                else
                    wall.sprite = this.gameLevel.add.image(wall.posX, wall.posY, this.mapDic[4].sheetName).setScale(1);

                    this.gameLevel.entities.push(wall);
            })
    }
}