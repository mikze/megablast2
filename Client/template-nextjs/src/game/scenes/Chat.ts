import { EventBus } from '../EventBus';
import { Scene } from 'phaser';
import { Player } from "./../Player/Player"
import { Connection } from '../SignalR/Connection';
import { MapGenerator } from './MapGenerator';
import { HubConnectionState } from '@microsoft/signalr';
import { Fire } from '../Player/Fire';
import { PlayerManager } from './PlayerManager';
import { Bonus } from '../Player/Bonus';
import { BonusModel } from '../Player/BonusModel';


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
  players: Player[];
  cameraSet: boolean
  entities : IEntity[]
  bonuses : Bonus[]


  constructor() {
    super('GameLevel');

    this.players = new Array<Player>();

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

  removeEntity(id: string)
  {
    this.entities.find(e => e.id === id)?.sprite.destroy();
  }

  killPlayer(id: string) {
    this.players.find(p => p.id === id)?.Dead();
  }

  resurrectPlayer(id: string) {
    this.players.find(p => p.id === id)?.Alive();
  }

  addFire(fires: Fire[]) {
    console.log(fires);
    fires.map(f => {
      f.image = this.add.image(f.posX, f.posY, "fire").setScale(0.8);
    })

    setTimeout(() => fires.map(f => f.image.destroy()) , 2000);
  }

  recMsg(userId: string, message: string) {
    this.players.find(p => p.id === userId)?.Say(message);
  }

  recMovePlayer(x: number, y: number, id: string) {
    let player = this.players.find(p => p.id === id);
    player?.Move(x, y);

    if(GameLevel.playerId === id && player !== undefined && !this.cameraSet)
    {
      this.camera.startFollow(player)
      this.cameraSet = true;
    }
  }

  nameChanged(newName: string, id: string) {
    let player = this.players.find(p => p.id === id);
    if (player !== undefined) {
      player.textName.setText(newName);
      player.name = newName;
    }
  }

  setPlayerId(id: string | null) {
    GameLevel.playerId = id;
}

bombPlanted(id :string)
{
  this.players.find(p => p.id === id)?.PlantBomb();
}

setBonus(bonus: BonusModel) {
  this.entities.push(new Bonus(bonus.id, bonus.posX, bonus.posY, bonus.bonusType, this));
}

  setPlayers() {
    console.log(PlayerManager.players)

    new Promise((r,c) => r(this.players.map(p=>{

      p.sprite.destroy();

    })))
    .then( () => {
    this.players = new Array<Player>();

    PlayerManager.players.map(p => 
      { 
      setTimeout(() => this.players.push(new Player(p.id, p.name, p.posX, p.posY, this, p.skin)), 50);})
      })
      
    }


  playerDisconnected(id: string) {
    this.players.find(p => p.id === id)?.textName.destroy();
    this.players.find(p => p.id === id)?.sprite.destroy();
    this.players = this.players.filter(p => p.id !== id);
  }

  

  preload() {
    console.log("Preload")
    Connection.gameLevel = this;
    this.cameraSet = false;
    this.setPlayers();
    this.setMap();
  }


  create() {
    console.log("Create")   
    this.camera = this.cameras.main;
    this.camera.setBackgroundColor("rgb(55, 87, 248)");

    this.keyA = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.LEFT);
    this.keyS = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.DOWN);
    this.keyD = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.RIGHT);
    this.keyW = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.UP);
    this.keySpace = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.SPACE)
    
    

    //this.background = this.add.image(512, 384, 'background');
    //this.background.setAlpha(0.5);

    EventBus.emit('current-scene-ready', this);
  }

  update(time: number, delta: number): void {
    if(Connection.connection.state === HubConnectionState.Connected)
    {
    if (this.keyA?.isDown)
      this.moveLeft();
    if (this.keyD?.isDown)
      this.moveRight();
    if (this.keyS?.isDown)
      this.moveDown();
    if (this.keyW?.isDown)
      this.moveUp();
    if(this.keyW?.isUp && this.keyS?.isUp && this.keyD?.isUp && this.keyA?.isUp)
      this.moveStop();
    if(this.keySpace !== undefined && Phaser.Input.Keyboard.JustDown(this.keySpace))
      this.plantBomb();
  }
  }

  changeScene() {
    Connection.connection.invoke("RestartGame");
  }
}
