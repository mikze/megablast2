import { EventBus } from '../EventBus';
import { Scene } from 'phaser';
import { Player } from "./../Player/Player"
import { Connection } from '../SignalR/Connection';
import { MapGenerator } from './MapGenerator';
import { HubConnectionState } from '@microsoft/signalr';


export class Chat extends Scene {

  camera: Phaser.Cameras.Scene2D.Camera;
  background: Phaser.GameObjects.Image;
  gameText: Phaser.GameObjects.Text;
  connection: Connection;
  keyA: Phaser.Input.Keyboard.Key | undefined;
  keyS: Phaser.Input.Keyboard.Key | undefined;
  keyD: Phaser.Input.Keyboard.Key | undefined;
  keyW: Phaser.Input.Keyboard.Key | undefined;
  playerId: string | null
  players: Player[];
  mapGenerator: MapGenerator;
  cameraSet: boolean


  constructor() {
    super('Chat');
    this.players = new Array<Player>();
  }

  setMap(map: number[][]) {
    this.mapGenerator.GenerateMap(map);
  }
  sendMsg(user: string, message: string) {
    console.log('send msg');
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

  recMsg(user: string, message: string) {
    this.gameText.text = user + " said: " + message
  }

  recMovePlayer(x: number, y: number, id: string) {
    let player = this.players.find(p => p.id === id);
    player?.Move(x, y);
    if(this.playerId === id && player !== undefined && !this.cameraSet)
    {
      console.log('set camera')
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

  setPlayers(players: Array<Player>) {
    players.map(p => {
      if (!this.players.some(pl => pl.id === p.id)) {
        let namee = this.add.text(p.x, p.y + 20, p.name, {
          fontFamily: 'Arial Black', fontSize: 38, color: '#ffffff',
          stroke: '#000000', strokeThickness: 8,
          align: 'center'
        }).setOrigin(0.5).setDepth(100);
        setTimeout(() => 
          {
            let newPlayer = new Player
            (p.id,
              p.name,
              p.x,
              p.y,
              this.add.sprite(50, 300, "playerSprite").setScale(1), namee);
            this.players.push(newPlayer)
          }, 50);
      }
      
    }
    )
  }

  playerDisconnected(id: string) {
    this.players.find(p => p.id === id)?.textName.destroy();
    this.players.find(p => p.id === id)?.sprite.destroy();
    this.players = this.players.filter(p => p.id !== id);
  }

  preload() {
    this.load.spritesheet("playerSprite", "assets/player.png",
      {
        frameHeight: 63,
        frameWidth: 63
      }
    );

    this.load.spritesheet("solidWall", "assets/SolidWall.png",
      {
        frameHeight: 63,
        frameWidth: 63
      }
    );

    this.connection = new Connection(this);
    this.mapGenerator = new MapGenerator(this);
  }

  create() {
    this.camera = this.cameras.main;
    this.camera.setBackgroundColor(0x00ff00);

    this.background = this.add.image(512, 384, 'background');
    this.background.setAlpha(0.5);

    this.gameText = this.add.text(512, 384, 'Gowno', {
      fontFamily: 'Arial Black', fontSize: 38, color: '#ffffff',
      stroke: '#000000', strokeThickness: 8,
      align: 'center'
    }).setOrigin(0.5).setDepth(100);

    this.keyA = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.A);
    this.keyS = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.S);
    this.keyD = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.D);
    this.keyW = this.input.keyboard?.addKey(Phaser.Input.Keyboard.KeyCodes.W);



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

    EventBus.emit('current-scene-ready', this);

  }

  update(time: number, delta: number): void {
    if(this.connection.connection.state === HubConnectionState.Connected)
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
  }
  }

  changeScene() {
    this.scene.start('GameOver');
  }
}
