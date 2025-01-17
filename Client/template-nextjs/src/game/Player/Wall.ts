export class Wall implements IEntity
{
    id : string
    posX : number
    posY : number
    destructible : Boolean
    sprite : Phaser.GameObjects.Image
    Polly: Phaser.Geom.Polygon
    render3d: boolean
    
    constructor(x : number, y : number, Destructible:  Boolean) 
    {
        this.posX = x;
        this.posY = y;
        this.destructible = Destructible;
        this.render3d  = false;
    }

    spriteName: string

    destroyed: boolean
}