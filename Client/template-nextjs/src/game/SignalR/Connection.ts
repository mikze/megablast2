import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { GameLevel } from "../scenes/GameLevel";
import { Wall } from "../Player/Wall";
import { Fire } from "../Player/Fire";
import { PlayerManager } from "../scenes/PlayerManager";
import { PlayerModel } from "../Player/PlayerModel";
import { BonusModel } from "../Player/BonusModel";
import { MapGenerator } from "../scenes/MapGenerator";
import { BombModel } from "../Player/BombModel";
import { Monster } from "../Player/Monster";
import { Lobby } from "../scenes/Lobby";
import { updateConfig } from '../../storesAndReducers/configReducer';
import { setAdmin } from '../../storesAndReducers/adminReducer';
import  configureStore  from '../../storesAndReducers/Store'
import { setGames } from "@/storesAndReducers/gamesReducer";
import router from "next/router";
import { Bullet } from "../Player/Bullet";
import { setPlayerStats } from "@/storesAndReducers/statsReducer";

interface Config {
    monsterAmount : number,
    monsterSpeed: number
    bombDelay: number
}

interface PlayerStats
{
    lives : number,
    bombs : number,
    range : number,
    speed : number
}

interface GameInfo {
    name: string,
    activePlayers : number,
    maxPlayers: number,
    passRequired: boolean
}

export class Connection {
    static connection: HubConnection;
    static gameLevel: GameLevel;
    static lobby: Lobby;
    private static URL = "http://192.168.100.100:5166/Chat";

    static CreateConnection() {
        Connection.connection = new HubConnectionBuilder()
            .withUrl(Connection.URL)
            .configureLogging(LogLevel.Information)
            .build();
        return new Promise((resolve, reject) => {
            resolve(Connection.start(Connection.connection));
        });
    }

    static emitMessage()
    {
        Connection.subscribers.map(s => s.RecMsg())
    }

    static registerMessageConsumer(scene :any)
    {
        Connection.subscribers.push(scene);
    }

    static subscribers: any[] = new Array<any>

    private static async start(connection: HubConnection) {
        try {
            await connection.start();
            console.log("SignalR Connected.");
            GameLevel.playerId = connection.connectionId;
            Connection.setUpConnectionHandlers(connection);
        } catch (err) {
            console.log(err);
            setTimeout(() => Connection.start(connection), 5000);
        }
    }

    private static setUpConnectionHandlers(connection: HubConnection): void {
        connection.onclose(async () => await Connection.start(connection));
        connection.on("ReceiveMessage", Connection.handleReceiveMessage);
        connection.on("MovePlayer", Connection.handleMovePlayer);
        connection.on("ServerIsFull", Connection.handleServerIsFull);
        connection.on("Connected", Connection.handleConnected);
        connection.on("SkinChanged", Connection.handleSkinChanged);
        connection.on("NameChanged", Connection.handleNameChanged);
        connection.on("Disconnected", Connection.handleDisconnected);
        connection.on("GetMap", Connection.handleGetMap);
        connection.on("BombPlanted", Connection.handleBombPlanted);
        connection.on("BombExplode", Connection.handleBombExplode);
        connection.on("SetBonus", Connection.handleSetBonus);
        connection.on("RemoveEntity", Connection.handleRemoveEntity);
        connection.on("Fires", Connection.handleFires);
        connection.on("KillPlayer", Connection.handleKillPlayer);
        connection.on("ResurrectPlayer", Connection.handleResurrectPlayer);
        connection.on("BackToLobby", Connection.handleBackToLobby);
        connection.on("GetMonsters", Connection.getMonsters);
        connection.on("MoveMonsters", Connection.moveMonsters)
        connection.on("GetConfig", Connection.getConfig)
        connection.on("IsAdmin", Connection.isAdmin)
        connection.on("RunningAllGames", Connection.runningAllGames)
        connection.on("JoinToGame", Connection.joinToGame)
        connection.on("GoToServerList", Connection.goToServerList)
        connection.on("BulletCreated", Connection.bulletCreated)
        connection.on("GetStats", Connection.getStats)
    }

    static getStats(stats : PlayerStats) {
        console.log("GetStats", stats);
        configureStore.dispatch(setPlayerStats(stats));
    }

    static bulletCreated(bullet: Bullet) {
        if (Connection.gameLevel !== undefined)
            Connection.gameLevel.addBullet(bullet)
    }

    static goToServerList() {
        new Promise((r,c) =>{  r(localStorage.setItem("gameName", "")); })
            .then(() => router.push('/'));
    }

    static joinToGame(gameName: string) {
        new Promise((r,c) =>{  r(localStorage.setItem("gameName", gameName)); })
            .then(() => router.push('/RunGame'));
    }

    static runningAllGames(games : GameInfo[]) {
        console.log("RunningAllGames", games);
        configureStore.dispatch(setGames({games: games}));
    }

    static isAdmin(isAdmin: boolean) {
        console.log("IsAdmin", isAdmin);
        configureStore.dispatch(setAdmin({isAdmin: isAdmin}));
    }
    
    static getConfig(config: Config) {
        console.log("GetConfig", config);
        configureStore.dispatch(updateConfig(config));
    }

    static InvokeConnection(action: string, ...args: any[]) {
        Connection.connection.invoke(action, ...args);
    }

    static moveMonsters(monsters: [Monster]) {
        if (Connection.gameLevel !== undefined) {
            Connection.gameLevel.moveEntities(monsters);
        }
    }

    static getMonsters(monsters: [Monster]) {
        if (Connection.gameLevel !== undefined) {
            Connection.gameLevel.setMonsters(monsters);
        }
    }

    private static handleReceiveMessage(id: string, user: string, message: string): void {
        if (Connection.gameLevel !== undefined)
            Connection.gameLevel.recMsg(id, message);
        if (Connection.lobby !== undefined)
            Connection.lobby.recMsg(user, message);
    }

    private static handleMovePlayer(obj: [PlayerModel]): void {
        if (Connection.gameLevel !== undefined) {
            obj.map(p => {
                Connection.gameLevel.recMovePlayer(p.posX, p.posY, p.id);
            });
        }
    }

    private static handleServerIsFull(): void {
        console.log("Server is Full");
    }

    private static handleConnected(players: PlayerModel[]): void {
        console.log("Connected ", players);
        PlayerManager.UpdatePlayers(players.map(item => new PlayerModel(item.id, item.name, item.posX, item.posY, item.skin)));
    }

    private static handleSkinChanged(id: string, newSkinName: string): void {
        PlayerManager.SkinChanged(newSkinName, id);
    }

    private static handleNameChanged(id: string, newName: string): void {
        PlayerManager.NameChanged(newName, id);
    }

    private static handleDisconnected(players: PlayerModel[]): void {
        PlayerManager.UpdatePlayers(players.map(item => new PlayerModel(item.id, item.name, item.posX, item.posY, item.skin)));
    }

    private static handleGetMap(map: Wall[]): void {
        MapGenerator.SetMap(map);
    }

    private static handleBombPlanted(bombModel: BombModel): void {
        if (Connection.gameLevel !== undefined)
            Connection.gameLevel.bombPlanted(bombModel);
    }

    private static handleBombExplode(bombModel: BombModel): void {
        if (Connection.gameLevel !== undefined)
            Connection.gameLevel.bombExplode(bombModel);
    }

    private static handleSetBonus(bonus: BonusModel): void {
        if (Connection.gameLevel !== undefined)
            Connection.gameLevel.setBonus(bonus);
    }

    private static handleRemoveEntity(id: string): void {
        if (Connection.gameLevel !== undefined)
            Connection.gameLevel.removeEntity(id);
    }

    private static handleFires(fires: Fire[]): void {
        if (Connection.gameLevel !== undefined)
            Connection.gameLevel.addFire(fires);
    }

    private static handleKillPlayer(id: string): void {
        if (Connection.gameLevel !== undefined) {
            console.log("KillPlayer", id);
            Connection.gameLevel.killPlayer(id);
        }
    }

    private static handleResurrectPlayer(id: string): void {
        if (Connection.gameLevel !== undefined) {
            console.log("ResurrectPlayer", id);
            Connection.gameLevel.resurrectPlayer(id);
        }
    }

    private static handleBackToLobby(id: string): void {
        if (Connection.gameLevel !== undefined) {
            PlayerManager.DestroySprites();
            Connection.gameLevel.scene.start('Lobby');
        }
    }
}