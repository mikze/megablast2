import { EventBus } from '../EventBus';
import { Scene } from 'phaser';
import { Connection } from '../SignalR/Connection';
import { PlayerManager } from './PlayerManager';
import { receiveMessage } from '../../storesAndReducers/chatReducer'
import  configureStore  from '../../storesAndReducers/Store'

const TEXT_STYLE = {
    fontFamily: 'Arial Black',
    fontSize: 30,
    color: '#ffffff',
    stroke: '#000000',
    strokeThickness: 8,
    align: 'center'
};

interface Config {
    monsterAmount : number,
    monsterSpeed: number
    bombDelay: number
}

export class Lobby extends Scene {

    private static loaded: boolean = false;
    private camera!: Phaser.Cameras.Scene2D.Camera;
    private background!: Phaser.GameObjects.Image;
    private gameText: Phaser.GameObjects.Text[] = [];

    constructor() {
        super('Lobby');
    }

    public RefreshPlayers(): void {
        Promise.resolve(this.gameText.map(text => text.destroy()))
            .then(() => this.setPlayerNames());
    }

    private async registerStart() {
        if (!Lobby.loaded) {
            Lobby.loaded = true;
            Connection.connection.on("Start", () => { this.scene.start('GameLevel'); });
        }
    }

    private async initializeConnection(): Promise<void> {
        await this.registerStart();
        PlayerManager.register(this);
        await Connection.connection.invoke("RestartGame");
        this.setPlayerNames();
        Connection.lobby = this;
        await Connection.connection.invoke("GetConfig");
        await Connection.connection.invoke("AmIAdmin");
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

    recMsg(user: string, message: string) {
        console.log(user + ": " + message);
        configureStore.dispatch(receiveMessage({message: message, username: user}));
    }

    setCfg(config : Config)
    {
        Connection.InvokeConnection("SetConfig", config);
    }

    sendMsg(user: string, message: string) {
        console.log(user + ": " + message);
        Connection.InvokeConnection("SendMessage", user, message);
    }
    
    private addPlayerText(name: string, x: number, y: number): void {
        this.gameText.push(this.add.text(x, y, name, TEXT_STYLE)
            .setOrigin(0.5).setDepth(100));
    }

    private skins = ['playerSprite', 'playerSprite2', 'playerSprite3']
    private skinIndex = 0;
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
            Connection.connection.invoke("ChangeSkin", this.skins[this.skinIndex === this.skins.length-1 ?  this.skinIndex : ++this.skinIndex]);
        });
        
        leftButton.on('pointerdown', () => {
            player.skinDown();
            Connection.connection.invoke("ChangeSkin", this.skins[this.skinIndex === 0 ? this.skinIndex : --this.skinIndex]);
        });
    }

    create(): void {
        this.initializeConnection();
        this.camera = this.cameras.main;
        this.camera.setBackgroundColor(0x00ff00);
        this.background = this.add.image(510, 384, 'background');
        //this.add.sprite(100, 140 + dist, player.skin).setScale(1);//backgroundtile
        EventBus.emit('current-scene-ready', this);
    }

    changeName(newName: string): void {
        Connection.connection.invoke("ChangeName", newName);
    }

    changeScene(): void {
        Connection.connection.invoke("Start");
    }
}
