import { Scene } from "phaser";

const BONUS_LOOKUP: { [key: number]: { imageKey: string, scale: number } } = {
    1: { imageKey: "bomb", scale: 0.8 },
    2: { imageKey: "dead", scale: 0.8 },
    3: { imageKey: "1up", scale: 0.2 },
    4: { imageKey: "fire", scale: 1 },
};

export class Bonus
{
    id : string
    posX : number
    posY : number
    scene: Scene
    destructible : Boolean
    sprite : Phaser.GameObjects.Image
    bonusType : number

    constructor(id : string ,x : number, y : number, bonusType : number, scene: Scene) 
    {
        this.id = id;
        this.posX = x;
        this.posY = y;
        this.scene = scene;
        this.bonusType = bonusType;

        this.createSprite();
        
    }
    
    private createSprite(): void {
        const bonusConfig = BONUS_LOOKUP[this.bonusType];
        if (bonusConfig) {
            this.sprite = this.scene.add.image(this.posX + 8, this.posY + 8, bonusConfig.imageKey).setScale(bonusConfig.scale);
        }
    }
}