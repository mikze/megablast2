import { Player } from "./Player"

export class PlayerModel {
    
    id : string
    name : string
    posX : number
    posY : number
    x : number
    y : number
    dead : boolean
    skin : string
    sprite : Phaser.GameObjects.Sprite

    constructor(id : string, name : string, x : number, y : number, skin : string) {
        this.id = id;
        this.posX = x;
        this.posY = y;
        this.name = name;
        this.skin = skin;
      }

      public skinUp()
      {
        //this.skin++;
      }

      public skinDown()
      {
        //this.skin--;
      }
}