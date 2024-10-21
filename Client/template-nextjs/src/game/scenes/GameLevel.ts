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
  static playerId: string | null
  cameraSet: boolean
  entities: IEntity[]
  bonuses: Bonus[]


  constructor() {
    super('GameLevel');

    this.entities = new Array<IEntity>();

    this.bonuses = new Array<Bonus>();
  }

  setMap() {
    MapGenerator.GenerateMap(this);
  }
  sendMsg(user: string, message: string) {
    Connection.connection.invoke("SendMessage", user, message);
  }

  changeName(newName: string) {
    Connection.connection.invoke("ChangeName", newName);
  }

  moveRight() {
    Connection.connection.invoke("MovePlayer", 0);
  }

  moveLeft() {
    Connection.connection.invoke("MovePlayer", 1);
  }

  moveUp() {
    Connection.connection.invoke("MovePlayer", 2);
  }

  moveDown() {
    Connection.connection.invoke("MovePlayer", 3);
  }

  moveStop() {
    Connection.connection.invoke("MovePlayer", 4);
  }

  plantBomb() {
    Connection.connection.invoke("PlantBomb");
  }

  backToLobby() {
    Connection.connection.invoke("BackToLobby");
  }

  removeEntity(id: string) {
    var entity = this.entities.find(e => e.id === id);
    if (entity !== undefined) {
      entity.sprite.destroy();
      if((entity as Bonus)?.bonusType === 3)
        this.sound.play("1up")
      let index = this.entities.indexOf(entity);
      if (index > -1) {
        this.entities.splice(index, 1);
      }
    }
  }

  killPlayer(id: string) {
    PlayerManager.players.find(p => p.id === id)?.player.Dead();
  }

  resurrectPlayer(id: string) {
    PlayerManager.players.find(p => p.id === id)?.player.Alive();
  }

  addFire(fires: Fire[]) {
    fires.map(f => {
      f.image = this.add.image(f.posX, f.posY, "fire").setScale(0.8);
    })

    setTimeout(() => fires.map(f => f.image.destroy()), 150);
  }

  recMsg(userId: string, message: string) {
    PlayerManager.players.find(p => p.id === userId)?.player.Say(message);
  }

  recMovePlayer(x: number, y: number, id: string) {
    let player = PlayerManager.players.find(p => p.id === id);
    player?.player?.Move(x, y);

    if (GameLevel.playerId === id && player?.player !== undefined && !this.cameraSet) {
      this.camera.startFollow(player.player)
      this.cameraSet = true;
    }
  }

  nameChanged(newName: string, id: string) {
    let player = PlayerManager.players.find(p => p.id === id);
    if (player !== undefined) {
      player.player.textName.setText(newName);
      player.player.name = newName;
    }
  }

  setPlayerId(id: string | null) {
    GameLevel.playerId = id;
  }

  bombPlanted(bombModel: BombModel) {
    let bomb = new Bomb(bombModel.posX, bombModel.posY, this, bombModel.id);
    this.entities.push(bomb);
  }

  bombExplode(bombModel: BombModel) {

    let bomb = this.entities.find(p => p.id === bombModel.id);
    if (bomb !== undefined) {
      this.sound.play("boom"); bomb.sprite.destroy();
      this.removeEntity(bombModel.id);
    }
  }

  setBonus(bonus: BonusModel) {
    this.entities.push(new Bonus(bonus.id, bonus.posX, bonus.posY, bonus.bonusType, this));;
  }

  setPlayers() {
    new Promise((r, c) => r(PlayerManager.DestroySprites()))
      .then(() => PlayerManager.players.map(p => {
        p.player = new Player(p.id, p.name, p.posX, p.posY, this, p.skin);
      }));
  }


  playerDisconnected(id: string) {
    PlayerManager.players.find(p => p.id === id)?.player.textName.destroy();
    PlayerManager.players.find(p => p.id === id)?.player.sprite.destroy();
    PlayerManager.players = PlayerManager.players.filter(p => p.id !== id);
  }

  preload() {
    console.log("Preload GameLevel")
    this.game.scene.scenes.map(s => s !== this ? s.scene.stop() : null);
    Connection.gameLevel = this;
    this.cameraSet = false;
    this.setPlayers();
    this.setMap();
  }

  create() {
    console.log("Create GameLevel")
    this.camera = this.cameras.main;
    this.camera.setBackgroundColor("rgb(55, 87, 248)");

    this.keyA = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.LEFT);
    this.keyS = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.DOWN);
    this.keyD = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.RIGHT);
    this.keyW = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.UP);
    this.keySpace = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.SPACE)

    EventBus.emit('current-scene-ready', this);
  }

  update(time: number, delta: number): void {
    if (Connection.connection.state === HubConnectionState.Connected) {
      if (this.keyA?.isDown)
        this.moveLeft();
      if (this.keyD?.isDown)
        this.moveRight();
      if (this.keyS?.isDown)
        this.moveDown();
      if (this.keyW?.isDown)
        this.moveUp();
      if (this.keyW?.isUp && this.keyS?.isUp && this.keyD?.isUp && this.keyA?.isUp)
        this.moveStop();
      if (this.keySpace !== undefined && Phaser.Input.Keyboard.JustDown(this.keySpace))
        this.plantBomb();
    }
  }
}
