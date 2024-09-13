import { PlayerModel } from "../Player/PlayerModel";

export class PlayerManager
{
    static players: PlayerModel[];

    static UpdatePlayers(players: PlayerModel[])
    {
        PlayerManager.players = players;
    }
}