import { TextField } from "@mui/material";
import { useState } from "react";

type Props = {
    sendMsg: Function
}

function Chat(props: Props) {
    // The sprite can only be moved in the MainMenu Scene
    //  References to the PhaserGame component (game and scene are exposed)
    
    const [messages] = useState<any>([])
    const [message, setMessage] = useState<string>("");

    return (
        <div className="chat-group">
            <div className="messages">
                {messages.map((m: { msg: any; }, index: number) => (
                    <div key={index}>{m.msg}</div>
                ))}
            </div>
            <div className="input-section">
                <TextField
                    value={message}
                    onChange={(e) => setMessage(e.target.value)}
                    id="fullWidth"
                    label="Filled"
                    variant="filled"
                />
                <button className="button" onClick={() => props.sendMsg(message)}>Send msg</button>
            </div>
        </div>)
}

export default Chat