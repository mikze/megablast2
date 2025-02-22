import { useState } from "react";
import { Connection } from "./game/SignalR/Connection";

const CreateNpc = () =>
    Connection.InvokeConnection("AddNpc");

function AddNpc() {
    // The sprite can only be moved in the MainMenu Scene
    //  References to the PhaserGame component (game and scene are exposed)

    return(
        <div>
            <button className="button" onClick={() => CreateNpc()}>Add npc</button>
        </div>)
}

export default AddNpc