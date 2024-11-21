import {Paper, TextField, Theme } from "@mui/material";
import { createStyles, makeStyles } from '@mui/styles'
import { useState } from "react";
import { useAppSelector } from './hooks'
import { MessageLeft, MessageRight } from "./Message";

type Props = {
    sendMsg: Function
}

const useStyles = makeStyles((theme: Theme) =>
    createStyles({
        paper: {
            width: "280px",
            height: "768px",
            maxWidth: "280px",
            maxHeight: "768px",
            display: "flex",
            alignItems: "center",
            flexDirection: "column",
            //position: "relative"
        },
        paper2: {
            width: "80vw",
            maxWidth: "500px",
            display: "flex",
            alignItems: "center",
            flexDirection: "column",
            position: "relative"
        },
        container: {
           // width: "100vw",
            //height: "100vh",
            //display: "flex",
            //alignItems: "center",
            //justifyContent: "center"
        },
        messagesBody: {
            width: "calc( 100% - 20px )",
            margin: 10,
            overflowY: "scroll",
            height: "calc( 100% - 80px )"
        }
    })
);

function Chat(props: Props) {
    // The sprite can only be moved in the MainMenu Scene
    //  References to the PhaserGame component (game and scene are exposed)
    
    const [message, setMessage] = useState<string>("");
    const count = useAppSelector((state) => state.chat.messages)
    // @ts-ignore
    const classes = useStyles();
    return (
        <div className="chat-group">
            <div className={classes.container}>
                <Paper className={classes.paper}>
                    <Paper id="style-1" className={classes.messagesBody}>
                        <div className="messages">
                            {count.map((m : {message : string, username : string}, index : string) => (
                                <div key={index}>
                                    <MessageLeft
                                        message={m.message}
                                        timestamp="MM/DD 00:00"
                                        photoURL="https://lh3.googleusercontent.com/a-/AOh14Gi4vkKYlfrbJ0QLJTg_DLjcYyyK7fYoWRpz2r4s=s96-c"
                                        displayName={m.username}
                                        avatarDisp={true}
                                    />
                                </div>
                            ))}
                        </div>
                    </Paper>
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
                </Paper>
            </div>
        </div>
    )
}

export default Chat