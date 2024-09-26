import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { GameLevel } from "../scenes/Chat";
import { Wall } from "../Player/Wall";
import { Fire } from "../Player/Fire";
import { PlayerManager } from "../scenes/PlayerManager";
import { PlayerModel } from "../Player/PlayerModel";
import { MapGenerator } from "../scenes/MapGenerator";
import { resolve } from "path";


export class Connection {
    static connection: HubConnection;
    static gameLevel: GameLevel

     static CreateConnection() {
        
        console.log("Create connection")
        Connection.connection = new HubConnectionBuilder()
            .withUrl("http://192.168.100.100:5166/Chat")
            .configureLogging(LogLevel.Information)
            .build();

        async function start(connection: HubConnection) {
            try {
                await connection.start();
                console.log("SignalR Connected.");
                GameLevel.playerId = connection.connectionId;

                Connection.connection.onclose(async () => {
                    await start(Connection.connection);
                });
        
                Connection.connection.on("ReceiveMessage", (user, message) => {
                    if (Connection.gameLevel !== undefined)
                        Connection.gameLevel.recMsg(user, message);
                });
        
                Connection.connection.on("MovePlayer", (obj: [PlayerModel]) => {
                    if (Connection.gameLevel !== undefined) {
                        obj.map(p => {
                            if(p.posX === undefined)
                                console.log(p.posX)
                            Connection.gameLevel.recMovePlayer(p.posX, p.posY, p.id);
                        }
                        );
                    }
                });
        
                Connection.connection.on("Connected", (players : PlayerModel[]) => {
                    console.log(players)
                    PlayerManager.UpdatePlayers(players.map(item => new PlayerModel(item.id, item.name, item.posX, item.posY, item.skin)));
                });

                Connection.connection.on("SkinChanged", (id: string, newSkinName: string) => {
                    PlayerManager.SkinChanged(newSkinName, id);
                });

                Connection.connection.on("NameChanged", (id: string, newName: string) => {
                    PlayerManager.NameChanged(newName, id);
                });
        
                Connection.connection.on("Disconnected", (players : PlayerModel[]) => {
                    PlayerManager.UpdatePlayers(players.map(item => new PlayerModel(item.id, item.name, item.posX, item.posY, item.skin)));
                })
        
                Connection.connection.on("GetMap", (map: Wall[]) => {
                    MapGenerator.SetMap(map);
                    })
        
                Connection.connection.on("BombPlanted", (id: string) => {
                    if (Connection.gameLevel !== undefined)
                        Connection.gameLevel.bombPlanted(id)
                });
        
                Connection.connection.on("RemoveEntity", (id: string) => {
                    if (Connection.gameLevel !== undefined)
                        Connection.gameLevel.removeEntity(id)
                });
        
                Connection.connection.on("Fires", (fires: Fire[]) => {
                    if (Connection.gameLevel !== undefined)
                        Connection.gameLevel.addFire(fires)
                });
        
                Connection.connection.on("KillPlayer", (id: string) => {
                    if (Connection.gameLevel !== undefined) {
                        console.log("KillPlayer", id)
                        Connection.gameLevel.killPlayer(id);
                    }
                });
        
                Connection.connection.on("ResurrectPlayer", (id: string) => {
                    if (Connection.gameLevel !== undefined)
                    {
                        console.log("ResurrectPlayerlayer", id)
                        Connection.gameLevel.resurrectPlayer(id);
                    }
                });
        
                Connection.connection.on("BackToLobby", (id: string) => {
                    if (Connection.gameLevel !== undefined)
                        Connection.gameLevel.scene.start('Lobby');
                });
            } catch (err) {
                console.log(err);
                setTimeout(start, 5000);
            }
        };

        return new Promise((resolve, reject) =>
        {
            resolve(start(Connection.connection));     
        });
    }

}