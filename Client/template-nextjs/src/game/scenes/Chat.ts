import { EventBus } from '../EventBus';
import { Scene } from 'phaser';
import { Player } from "./../Player/Player"
import { Connection } from '../SignalR/Connection';
import { MapGenerator } from './MapGenerator';
import { HubConnectionState } from '@microsoft/signalr';


export class GameLevel extends Scene {

  camera: Phaser.Cameras.Scene2D.Camera;
  background: Phaser.GameObjects.Image;
  connection: Connection;
  keyA: Phaser.Input.Keyboard.Key | undefined;
  keyS: Phaser.Input.Keyboard.Key | undefined;
  keyD: Phaser.Input.Keyboard.Key | undefined;
  keyW: Phaser.Input.Keyboard.Key | undefined;
  keySpace: Phaser.Input.Keyboard.Key | undefined;
  playerId: string | null
  players: Player[];
  mapGenerator: MapGenerator;
  cameraSet: boolean


  constructor() {
    super('GameLevel');
    this.players = new Array<Player>();
  }

  setMap(map: number[][]) {
    this.mapGenerator.GenerateMap(map);
  }
  sendMsg(user: string, message: string) {
    this.connection.connection.invoke("SendMessage", user, message);
  }

  changeName(newName: string) {
    this.connection.connection.invoke("ChangeName", newName);
  }

  moveRight() {
    this.connection.connection.invoke("MovePlayer", 0);
  }

  moveLeft() {
    this.connection.connection.invoke("MovePlayer", 1);
  }

  moveUp() {
    this.connection.connection.invoke("MovePlayer", 2);
  }

  moveDown() {
    this.connection.connection.invoke("MovePlayer", 3);
  }

  moveStop() {
    this.connection.connection.invoke("MovePlayer", 4);
  }

  plantBomb() {
    this.connection.connection.invoke("PlantBomb");
  }

  recMsg(userId: string, message: string) {
    this.players.find(p => p.id === userId)?.Say(message);
  }

  recMovePlayer(x: number, y: number, id: string) {
    let player = this.players.find(p => p.id === id);
    player?.Move(x, y);
    if(this.playerId === id && player !== undefined && !this.cameraSet)
    {
      this.camera.startFollow(player)
      this.cameraSet = true;
    }
  }

  nameChanged(newName : string, id : string){
    this.players.find(p => p.id === id)?.textName.setText(newName);
  }

  setPlayerId(id: string | null) {
    this.playerId = id;
}

bombPlanted(id :string)
{
  this.players.find(p => p.id === id)?.PlantBomb();
}

  setPlayers(players: Array<Player>) {
    players.map(p => {
      if (!this.players.some(pl => pl.id === p.id))
        setTimeout(() => this.players.push(new Player(p.id, p.name, p.x, p.y, this)), 50);
    }
    )
  }

  playerDisconnected(id: string) {
    this.players.find(p => p.id === id)?.textName.destroy();
    this.players.find(p => p.id === id)?.sprite.destroy();
    this.players = this.players.filter(p => p.id !== id);
  }

  preload() {
    this.asyncLoad();
  }

  async asyncLoad()
  {
    this.connection = await new Connection(this);
    this.mapGenerator = await new MapGenerator(this);
  }

  create() {
    this.camera = this.cameras.main;
    this.camera.setBackgroundColor("rgb(55, 87, 248)");

    this.keyA = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.LEFT);
    this.keyS = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.DOWN);
    this.keyD = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.RIGHT);
    this.keyW = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.UP);
    this.keySpace = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.SPACE)
    

    this.anims.create({
      key: "walkRight",
      frames: this.anims.generateFrameNumbers("playerSprite", {
        frames: [8, 9, 10, 11],
      }),
      frameRate: 10,
      repeat: -1,
    });

    this.anims.create({
      key: "walkLeft",
      frames: this.anims.generateFrameNumbers("playerSprite", {
        frames: [4, 5, 6, 7],
      }),
      frameRate: 10,
      repeat: -1,
    });

    this.anims.create({
      key: "walkUp",
      frames: this.anims.generateFrameNumbers("playerSprite", {
        frames: [12, 13, 14, 15],
      }),
      frameRate: 20,
    });

    this.anims.create({
      key: "walkDown",
      frames: this.anims.generateFrameNumbers("playerSprite", {
        frames: [0, 1, 2, 3],
      }),
      frameRate: 20,
    });

    this.anims.create({
      key: "turn",
      frames: [{ key: "playerSprite", frame: 1 }],
      frameRate: 20,
    });

    this.background = this.add.image(512, 384, 'background');
    this.background.setAlpha(0.5);

    EventBus.emit('current-scene-ready', this);

  }

  update(time: number, delta: number): void {
    if(this.connection?.connection.state === HubConnectionState.Connected)
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
    if(this.keySpace?.isDown)
      this.plantBomb();
  }
  }

  changeScene() {
    this.scene.start('GameOver');
  }
}
