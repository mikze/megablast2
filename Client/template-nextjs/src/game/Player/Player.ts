  
  export class Player {

    id : string
    name : string
    x : number
    y : number
    textName : Phaser.GameObjects.Text
    sprite : Phaser.GameObjects.Sprite
  
    constructor(id : string, name : string, x : number, y : number, sprite : any, textName: Phaser.GameObjects.Text) {
      this.id = id;
      this.name = name;
      this.x = x;
      this.y = y;
      this.sprite = sprite;
      this.sprite.x = x;
      this.sprite.y = y;
      this.textName = textName
      this.textName.x = x;
      this.textName.y = y - 20;
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
