import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { GameLevel } from "../scenes/GameLevel";
import { Wall } from "../Player/Wall";
import { Fire } from "../Player/Fire";
import { PlayerManager } from "../scenes/PlayerManager";
import { PlayerModel } from "../Player/PlayerModel";
import { BonusModel } from "../Player/BonusModel";
import { MapGenerator } from "../scenes/MapGenerator";
import { BombModel } from "../Player/BombModel";


export class Connection {
    static connection: HubConnection;
    static gameLevel: GameLevel;

    private static URL = "http://192.168.100.25:5166/Chat";
    private static LOG_MESSAGE_CONNECTION = "Create connection";

    static CreateConnection() {
        console.log(Connection.LOG_MESSAGE_CONNECTION);
        Connection.connection = new HubConnectionBuilder()
            .withUrl(Connection.URL)
            .configureLogging(LogLevel.Information)
            .build();
        return new Promise((resolve, reject) => {
            resolve(Connection.start(Connection.connection));
        });
    }

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
    }

    private static handleReceiveMessage(user: string, message: string): void {
        if (Connection.gameLevel !== undefined)
            Connection.gameLevel.recMsg(user, message);
    }

    private static handleMovePlayer(obj: [PlayerModel]): void {
        if (Connection.gameLevel !== undefined) {
            obj.map(p => {
                if (p.posX === undefined)
                    console.log(p.posX);
                Connection.gameLevel.recMovePlayer(p.posX, p.posY, p.id);
            });
        }
    }

    private static handleServerIsFull(): void {
        console.log("Server is Full");
    }

    private static handleConnected(players: PlayerModel[]): void {
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