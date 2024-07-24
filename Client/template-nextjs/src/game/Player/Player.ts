class Player {

    id : string
    name : string
    x : number
    y : number
    textName : Phaser.GameObjects.Text
    sprite : Phaser.GameObjects.Sprite
  
    constructor(id : string, name : string, x : number, y : number, sprite : any) {
      this.id = id;
      this.name = name;
      this.x = x;
      this.y = y;
      this.sprite = sprite;
    }
  }