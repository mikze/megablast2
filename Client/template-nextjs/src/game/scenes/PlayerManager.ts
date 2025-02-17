import { PlayerModel } from "../Player/PlayerModel";

export class PlayerManager {
    static SkinChanged(newSkinName: string, id: string) {
        new Promise((r) => r(PlayerManager.changeSkin(newSkinName, id))).then(() => PlayerManager.emit());
    }
    static changeSkin(newSkinName: string, id: string) {
        console.log("skin changed", newSkinName, id);
        let player = this.players.find(p => p.id === id);
        if (player !== undefined) {
            player.skin = newSkinName;
        }
    }

    static DestroySprites()
    {
        PlayerManager.players.map(p => 
        {
                p?.sprite?.destroy();
        })
    }

    static NameChanged(newName: string, id: string) {     
        new Promise((r) => r(PlayerManager.changeName(newName, id))).then(() => PlayerManager.emit());
    }

    static players: PlayerModel[];

    static UpdatePlayers(players: PlayerModel[]) {
        new Promise((r) => r(PlayerManager.setPlayers(players))).then(() => PlayerManager.emit());
    }
    static setPlayers(players: PlayerModel[])
    {
        if(PlayerManager.players !== undefined)
            new Promise((r) => 
        {
            r(PlayerManager.DestroySprites());
        })
        .then(() => {
            PlayerManager.players = players;
        });
        else
            PlayerManager.players = players;
    }

    static changeName(newName: string, id: string)
    {
        console.log("name changed", newName, id);
        let player = this.players.find(p => p.id === id);
        if (player !== undefined) {
            player.name = newName;
        }
    }
    static emit()
    {
        PlayerManager.subscribers.map(s => s.RefreshPlayers())
    }

    static register(scene :any)
    {
        PlayerManager.subscribers.push(scene);
    }

    static subscribers: any[] = new Array<any>
}