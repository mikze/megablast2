import { EventBus } from '../EventBus';
import { Scene } from 'phaser';
import { Connection } from '../SignalR/Connection';
import { HubConnectionState } from '@microsoft/signalr';
import { PlayerManager } from './PlayerManager';

export class Lobby extends Scene {
    camera: Phaser.Cameras.Scene2D.Camera;
    background: Phaser.GameObjects.Image;
    gameText: Phaser.GameObjects.Text[];

    constructor() {
        super('Lobby');
        this.gameText = new Array<Phaser.GameObjects.Text>()
    }

    preload() {
        this.load.audio("boom", [ 'assets/audio/boom.mp3', 'audio/boom.ogg', 'audio/boom.m4a' ]);   
        this.load.audio("kill", [ 'assets/audio/kill.mp3' ]);  
    }

     RefreshPlayers()
     {
        console.log("RefreshPlayers")
        new Promise((r,c) => r(this.gameText.map(t => t.destroy()))).then(() => this.SetNamePlayer());
     }

     CreateConn() {
        if (Connection.connection === undefined || Connection.connection.state !== HubConnectionState.Connected) 
            {
                console.log("Stage 1: Create Connection");
                Connection.CreateConnection()
                .then(() => {
                    console.log("Stage 2: Register Start")
                    new Promise((resolve, reject) => 
                        resolve(Connection.connection.on("Start", () => this.scene.start('GameLevel'))))
                    .then((r) => {
                        console.log("Stage 3: Restart lobby")
                        PlayerManager.register(this);
                        new Promise((resolve, reject) => resolve(Connection.connection.invoke("RestartGame"))).then(() => this.SetNamePlayer())
                    })
                })
                .catch(() => console.log("Error xDD"));         
            }
            else
            {
            this.SetNamePlayer();
        }
    }

    SetNamePlayer() {
      if(PlayerManager.players !== undefined)
      {
        let dist = 0;
        PlayerManager.players.map(p => {
            this.gameText.push(this.add.text(100, 84 + dist, p.name, {
                fontFamily: 'Arial Black', fontSize: 30, color: '#ffffff',
                stroke: '#000000', strokeThickness: 8,
                align: 'center'
            }).setOrigin(0.5).setDepth(100))
            
            p.sprite = this.add.sprite(100, 140 + dist, p.skin).setScale(1);

            if (p.id === Connection.connection.connectionId) {
                let prawo = this.add.text(100 + 130, 140 + dist, ">", {
                    fontFamily: 'Arial Black', fontSize: 20, color: '#ffffff',
                    stroke: '#000000', strokeThickness: 8,
                    align: 'center'
                }).setOrigin(0.5).setDepth(100)

                let lewo = this.add.text(100 + 80 , 140 + dist, "<", {
                    fontFamily: 'Arial Black', fontSize: 20, color: '#ffffff',
                    stroke: '#000000', strokeThickness: 8,
                    align: 'center'
                }).setOrigin(0.5).setDepth(100)

                prawo.setInteractive();
                prawo.on('pointerover', () => { console.log('prawo pointerover', p); });
                prawo.on('pointerdown', () => { 
                    p.skinUp(); 
                    console.log(p.skin); 
                    Connection.connection.invoke("ChangeSkin", "playerSprite2");
                });

                lewo.setInteractive();
                lewo.on('pointerover', () => { console.log('lewo pointerover' , p); });
                lewo.on('pointerdown', () => { 
                    p.skinDown(); 
                    console.log(p.skin); 
                    Connection.connection.invoke("ChangeSkin", "playerSprite");
                });
            }

            dist += 120;
            })
  }}

  
    create() {

        this.CreateConn();
        this.camera = this.cameras.main;
        this.camera.setBackgroundColor(0x00ff00);

        this.background = this.add.image(512, 384, 'background');
        this.background.setAlpha(0.5);        

        EventBus.emit('current-scene-ready', this);
    }

    changeName(newName: string) {
        Connection.connection.invoke("ChangeName", newName);
      }
      
    changeScene() {
        Connection.connection.invoke("Start");
    }
}
