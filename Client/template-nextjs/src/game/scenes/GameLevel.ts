import { EventBus } from '../EventBus';
import { Scene } from 'phaser';
import { Player } from "../Player/Player"
import { Connection } from '../SignalR/Connection';
import { MapGenerator } from './MapGenerator';
import { HubConnectionState } from '@microsoft/signalr';
import { Fire } from '../Player/Fire';
import { PlayerManager } from './PlayerManager';
import { Bonus } from '../Player/Bonus';
import { BonusModel } from '../Player/BonusModel';
import { BombModel } from '../Player/BombModel';
import { Bomb } from '../Player/Bomb';


export class GameLevel extends Scene {
    camera: Phaser.Cameras.Scene2D.Camera;
    background: Phaser.GameObjects.Image;
    connection: Connection;
    keyA: Phaser.Input.Keyboard.Key | undefined;
    keyS: Phaser.Input.Keyboard.Key | undefined;
    keyD: Phaser.Input.Keyboard.Key | undefined;
    keyW: Phaser.Input.Keyboard.Key | undefined;
    keySpace: Phaser.Input.Keyboard.Key | undefined;
    static playerId: string | null;
    cameraSet: boolean;
    entities: IEntity[];
    bonuses: Bonus[];

    constructor() {
        super('GameLevel');
        this.entities = [];
        this.bonuses = [];
    }

    private invokeConnection(action: string, ...args: any[]) {
        Connection.connection.invoke(action, ...args);
    }

    private getPlayerById(id: string) {
        return PlayerManager.players.find(p => p.id === id);
    }

    private handleKeyInputs() {
        if (this.keyA?.isDown) this.moveLeft();
        if (this.keyD?.isDown) this.moveRight();
        if (this.keyS?.isDown) this.moveDown();
        if (this.keyW?.isDown) this.moveUp();
        if (this.keyW?.isUp && this.keyS?.isUp && this.keyD?.isUp && this.keyA?.isUp) this.moveStop();
        if (this.keySpace && Phaser.Input.Keyboard.JustDown(this.keySpace)) this.plantBomb();
    }

    private destroyEntity(entity: IEntity) {
        entity.sprite.destroy();
        if ((entity as Bonus)?.bonusType === 3) this.sound.play("1up");
        const index = this.entities.indexOf(entity);
        if (index > -1) this.entities.splice(index, 1);
    }

    setMap() {
        MapGenerator.GenerateMap(this);
    }

    sendMsg(user: string, message: string) {
        this.invokeConnection("SendMessage", user, message);
    }

    changeName(newName: string) {
        this.invokeConnection("ChangeName", newName);
    }

    moveRight() {
        this.invokeConnection("MovePlayer", 0);
    }

    moveLeft() {
        this.invokeConnection("MovePlayer", 1);
    }

    moveUp() {
        this.invokeConnection("MovePlayer", 2);
    }

    moveDown() {
        this.invokeConnection("MovePlayer", 3);
    }

    moveStop() {
        this.invokeConnection("MovePlayer", 4);
    }

    plantBomb() {
        this.invokeConnection("PlantBomb");
    }

    backToLobby() {
        this.invokeConnection("BackToLobby");
    }

    removeEntity(id: string) {
        const entity = this.entities.find(e => e.id === id);
        if (entity) this.destroyEntity(entity);
    }

    killPlayer(id: string) {
        this.getPlayerById(id)?.player.Dead();
    }

    resurrectPlayer(id: string) {
        this.getPlayerById(id)?.player.Alive();
    }

    addFire(fires: Fire[]) {
        fires.forEach(f => {
            f.image = this.add.image(f.posX, f.posY, "fire").setScale(0.8);
        });
        setTimeout(() => fires.forEach(f => f.image.destroy()), 150);
    }

    recMsg(userId: string, message: string) {
        this.getPlayerById(userId)?.player.Say(message);
    }

    recMovePlayer(x: number, y: number, id: string) {
        const player = this.getPlayerById(id);
        player?.player?.Move(x, y);
        if (GameLevel.playerId === id && player?.player && !this.cameraSet) {
            this.camera.startFollow(player.player);
            this.cameraSet = true;
        }
    }

    nameChanged(newName: string, id: string) {
        const player = this.getPlayerById(id);
        if (player) {
            player.player.name = newName;
            player.player.textName.setText(newName);
        }
    }

    setPlayerId(id: string | null) {
        GameLevel.playerId = id;
    }

    bombPlanted(bombModel: BombModel) {
        this.entities.push(new Bomb(bombModel.posX, bombModel.posY, this, bombModel.id));
    }

    bombExplode(bombModel: BombModel) {
        const bomb = this.entities.find(p => p.id === bombModel.id);
        if (bomb) {
            this.sound.play("boom");
            bomb.sprite.destroy();
            this.removeEntity(bombModel.id);
        }
    }

    setBonus(bonus: BonusModel) {
        this.entities.push(new Bonus(bonus.id, bonus.posX, bonus.posY, bonus.bonusType, this));
    }

    setPlayers() {
        new Promise((resolve) => resolve(PlayerManager.DestroySprites()))
            .then(() => PlayerManager.players.forEach(p => {
                p.player = new Player(p.id, p.name, p.posX, p.posY, this, p.skin);
            }));
    }

    playerDisconnected(id: string) {
        const player = this.getPlayerById(id);
        if (player) {
            player.player.textName.destroy();
            player.player.sprite.destroy();
            PlayerManager.players = PlayerManager.players.filter(p => p.id !== id);
        }
    }

    preload() {
        console.log("Preload GameLevel");
        this.game.scene.scenes.forEach(s => s !== this && s.scene.stop());
        Connection.gameLevel = this;
        this.cameraSet = false;
        this.setPlayers();
        this.setMap();
    }

    create() {
        console.log("Create GameLevel");
        this.camera = this.cameras.main;
        this.camera.setBackgroundColor("rgb(55, 87, 248)");
        this.keyA = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.LEFT);
        this.keyS = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.DOWN);
        this.keyD = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.RIGHT);
        this.keyW = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.UP);
        this.keySpace = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.SPACE);
        EventBus.emit('current-scene-ready', this);
    }

    update(time: number, delta: number): void {
        if (Connection.connection.state === HubConnectionState.Connected) {
            this.handleKeyInputs();
        }
    }
}
