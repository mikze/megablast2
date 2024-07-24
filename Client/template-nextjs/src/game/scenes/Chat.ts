import { EventBus } from '../EventBus';
import { Scene } from 'phaser';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr'


export class Chat extends Scene {
    camera: Phaser.Cameras.Scene2D.Camera;
    background: Phaser.GameObjects.Image;
    gameText: Phaser.GameObjects.Text;
    connection: HubConnection;
    keyA: Phaser.Input.Keyboard.Key | undefined;
    keyS: Phaser.Input.Keyboard.Key | undefined;
    keyD: Phaser.Input.Keyboard.Key | undefined;
    keyW: Phaser.Input.Keyboard.Key | undefined;
    player: Player

    constructor() {
        super('Chat');
    }

    f() {
        this.connection = new HubConnectionBuilder()
            .withUrl("http://192.168.100.100:5166/Chat")
            .configureLogging(LogLevel.Information)
            .build();

        async function start(connection : HubConnection ) {
            try {
                await connection.start();
                console.log("SignalR Connected.");
            } catch (err) {
                console.log(err);
                setTimeout(start, 5000);
            }
        };

        this.connection.onclose(async () => {
            await start(this.connection);
        });

        this.connection.on("ReceiveMessage", (user, message) => {
            console.log(user + " said: " + message);

            this.gameText.text = user + " said: " + message;
        });

        this.connection.on("MoveText", (x :integer) => {
            this.gameText.x = this.gameText.x + x;
        });

        this.connection.on("Connected", (user, message) => {
            console.log("BLABLA2");
        });

        // Start the connection.
        start(this.connection);
    }

    sendMsg(user: string, message: string)
    {
        console.log('send msg');
        this.connection.invoke("SendMessage", user, message);
    }

    moveText()
    {
        this.connection.invoke("MoveText");
    }

    preload()
    {
        console.log('pre load');
        this.load.spritesheet("playerSprite", "assets/player.png",
            {
                frameHeight: 63,
                frameWidth: 63
            }
        );
        this.f();
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

        this.player = new Player("a", "mikze", 30, 30,this.add.sprite(50,300,"playerSprite").setScale(1));

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
        if (this.keyA?.isDown) 
            this.player.sprite.play({ key: "walkLeft", repeat: 1 }, true);
        if (this.keyD?.isDown) 
            this.player.sprite.play({ key: "walkRight", repeat: 1 }, true);
        if (this.keyS?.isDown) 
            this.player.sprite.play({ key: "walkDown", repeat: 1 }, true);
        if (this.keyW?.isDown) 
            this.player.sprite.play({ key: "walkUp", repeat: 1 }, true);
        
    }

    changeScene() {
        this.scene.start('GameOver');
    }
}
