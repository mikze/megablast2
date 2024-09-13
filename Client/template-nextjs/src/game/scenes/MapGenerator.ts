import { Wall } from "../Player/Wall";
import { GameLevel } from "./Chat";

export class MapGenerator {
    static map: Wall[]
    static mapDic: { [id: integer] : {sheetName: string, x : integer}}

    static SetMap(map: Wall[])
    {
        MapGenerator.map = map;
    }

    static GenerateMap(gameLevel :GameLevel) {

        MapGenerator.mapDic = {};
        MapGenerator.mapDic[0] = { sheetName: "otsp_tiles_01", x: 144};
        MapGenerator.mapDic[1] = { sheetName: "solidWall", x: -1};
        MapGenerator.mapDic[2] = { sheetName: "otsp_town_01", x: 11};
        MapGenerator.mapDic[3] = { sheetName: 'otsp_town_01', x: 7};
        MapGenerator.mapDic[4] = { sheetName: "destructiveWall", x: -1};

        gameLevel.entities.map(
            e => {
                e.sprite.destroy();
            }
        );

        gameLevel.entities = new Array<IEntity>();
        
        MapGenerator.map.map(wall => {
                if(!wall.destructible)
                    wall.sprite = gameLevel.add.image(wall.posX, wall.posY, MapGenerator.mapDic[1].sheetName, MapGenerator.mapDic[1].x).setScale(1);
                else
                    wall.sprite = gameLevel.add.image(wall.posX, wall.posY, MapGenerator.mapDic[4].sheetName).setScale(1);

                    gameLevel.entities.push(wall);
            })
    }
}