import { EventBus } from '../EventBus';
import { Scene } from 'phaser';
import { Connection } from '../SignalR/Connection';
import { HubConnectionState } from '@microsoft/signalr';

export class Lobby extends Scene {
    camera: Phaser.Cameras.Scene2D.Camera;
    background: Phaser.GameObjects.Image;
    gameText: Phaser.GameObjects.Text;

    constructor() {
        super('Lobby');
    }

    preload() {
        this.CreateConn();
    }
    async CreateConn() {
        if (Connection.connection === undefined || Connection.connection.state !== HubConnectionState.Connected) {
            await Connection.CreateConnection();

            await Connection.connection.on("Start", (id: string) => {
                this.scene.start('GameLevel');
            })

            setTimeout(() => Connection.connection.invoke("RestartGame"), 500);
        }
    }  

    create() {
        this.camera = this.cameras.main;
        this.camera.setBackgroundColor(0x00ff00);

        this.background = this.add.image(512, 384, 'background');
        this.background.setAlpha(0.5);

        this.gameText = this.add.text(512, 384, 'Lobby', {
            fontFamily: 'Arial Black', fontSize: 38, color: '#ffffff',
            stroke: '#000000', strokeThickness: 8,
            align: 'center'
        }).setOrigin(0.5).setDepth(100);

        EventBus.emit('current-scene-ready', this);
    }

    changeName(newName: string) {
        Connection.connection.invoke("ChangeName", newName);
      }
      
    changeScene() {
        Connection.connection.invoke("Start");
    }
}
