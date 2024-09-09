import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { GameLevel } from "../scenes/Chat";
import { Wall } from "../Player/Wall";

type PlayerModel
= 
{
    posX :number, 
    posY: number, 
    id: string
}
export class Connection
{
    connection: HubConnection;

    constructor(chat: GameLevel)
    {
        this.connection = new HubConnectionBuilder()
            .withUrl("http://192.168.100.100:5166/Chat")
            .configureLogging(LogLevel.Information)
            .build();

        async function start(connection : HubConnection ) {
            try {
                await connection.start();
                console.log("SignalR Connected.");
                chat.setPlayerId(connection.connectionId);
            } catch (err) {
                console.log(err);
                setTimeout(start, 5000);
            }
        };

        this.connection.onclose(async () => {
            await start(this.connection);
        });

        this.connection.on("ReceiveMessage", (user, message) => {
            chat.recMsg(user,message);
        });

        this.connection.on("MovePlayer", (obj : [PlayerModel]) => {
            
            obj.map( p => 
                { 
                    chat.recMovePlayer(p.posX, p.posY, p.id);}
                );
        });

        this.connection.on("Connected", (players, id) => {
            chat.setPlayers(players);
        });

        this.connection.on("NameChanged", (id : string, newName: string) => {
            chat.nameChanged(newName, id);
        });

        this.connection.on("Disconnected", (id : string) => {
            chat.playerDisconnected(id);
        })

        this.connection.on("GetMap", (map : Wall[]) => {
            chat.setMap(map);
        })

        this.connection.on("BombPlanted", (id : string) => {
            chat.bombPlanted(id)
        });

        this.connection.on("RemoveEntity", (id : string) => {
            chat.removeEntity(id)
        });

        // Start the connection.
        start(this.connection);
    }
}