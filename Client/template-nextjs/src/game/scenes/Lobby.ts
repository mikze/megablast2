import { EventBus } from '../EventBus';
import { Scene } from 'phaser';
import { Connection } from '../SignalR/Connection';
import { PlayerManager } from './PlayerManager';

const TEXT_STYLE = {
    fontFamily: 'Arial Black',
    fontSize: 30,
    color: '#ffffff',
    stroke: '#000000',
    strokeThickness: 8,
    align: 'center'
};

export class Lobby extends Scene {
    private static loaded: boolean = false;
    private camera!: Phaser.Cameras.Scene2D.Camera;
    private background!: Phaser.GameObjects.Image;
    private gameText: Phaser.GameObjects.Text[] = [];

    constructor() {
        super('Lobby');
    }

    private RefreshPlayers(): void {
        Promise.resolve(this.gameText.map(text => text.destroy()))
            .then(() => this.setPlayerNames());
    }

    private registerStart(): void {
        if (!Lobby.loaded) {
            Lobby.loaded = true;
            Connection.connection.on("Start", () => { this.scene.start('GameLevel'); });
        }
    }

    private async initializeConnection(): Promise<void> {
        console.log("Stage 2: Register Start");
        await this.registerStart();
        console.log("Stage 3: Restart lobby");
        PlayerManager.register(this);
        await Connection.connection.invoke("RestartGame");
        this.setPlayerNames();
        return Promise.resolve();
    }

    private setPlayerNames(): void {
        if (PlayerManager.players !== undefined) {
            let dist = 0;
            PlayerManager.players.map(player => {
                this.addPlayerText(player.name, 100, 84 + dist);
                player.sprite = this.add.sprite(100, 140 + dist, player.skin).setScale(1);
                if (player.id === Connection.connection.connectionId) {
                    this.addPlayerControls(player, dist);
                }
                dist += 120;
            });
        }
    }

    private addPlayerText(name: string, x: number, y: number): void {
        this.gameText.push(this.add.text(x, y, name, TEXT_STYLE)
            .setOrigin(0.5).setDepth(100));
    }

    private addPlayerControls(player: any, dist: number): void {
        const rightButton = this.add.text(230, 140 + dist, ">", TEXT_STYLE)
            .setScale(20 / 30)
            .setOrigin(0.5).setDepth(100)
            .setInteractive();

        const leftButton = this.add.text(180, 140 + dist, "<", TEXT_STYLE)
            .setScale(20 / 30)
            .setOrigin(0.5).setDepth(100)
            .setInteractive();
        
        rightButton.on('pointerdown', () => {
            player.skinUp();
            console.log(player.skin);
            Connection.connection.invoke("ChangeSkin", "playerSprite2");
        });

        leftButton.on('pointerover', () => { console.log('Left button hover', player); });
        leftButton.on('pointerdown', () => {
            player.skinDown();
            console.log(player.skin);
            Connection.connection.invoke("ChangeSkin", "playerSprite");
        });
    }

    create(): void {
        console.log('Lobby start');
        this.initializeConnection();
        this.camera = this.cameras.main;
        this.camera.setBackgroundColor(0x00ff00);
        this.background = this.add.image(512, 384, 'background').setAlpha(0.5);
        EventBus.emit('current-scene-ready', this);
    }

    changeName(newName: string): void {
        Connection.connection.invoke("ChangeName", newName);
    }

    changeScene(): void {
        console.log("Connection start");
        Connection.connection.invoke("Start");
    }
}
