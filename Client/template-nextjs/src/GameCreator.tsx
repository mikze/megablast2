import { BrowserRouter, Routes, Route } from "react-router";
import { Connection } from "./game/SignalR/Connection";
import { useState } from "react";
import {Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, TextField } from "@mui/material";


function GameCreator() {
    const CreateGame = (gameName : string) =>
        Connection.InvokeConnection("CreateGame", gameName);

    const GetGames = () =>
        Connection.InvokeConnection("GetRunningAllGames");

    const [gameName, setGameName] = useState("");

    const [open, setOpen] = useState(true);

    const handleClickOpen = () => {
        setOpen(true);
    };

    const handleClose = () => {
        setOpen(false);
    };
    

    return (
        <>
            <Dialog
                open={open}
                onClose={handleClose}
                PaperProps={{
                    component: 'form',
                    onSubmit: (event: React.FormEvent<HTMLFormElement>) => {
                        event.preventDefault();
                        const formData = new FormData(event.currentTarget);
                        const formJson = Object.fromEntries((formData as any).entries());
                        const email = formJson.email;
                        console.log(email);
                        handleClose();
                    },
                }}
            >
                <DialogTitle>Create new game</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Enter lobby's name.
                    </DialogContentText>
                    <TextField
                        autoFocus
                        required
                        margin="dense"
                        id="name"
                        name="game"
                        label="name"
                        type="name"
                        fullWidth
                        variant="standard"
                        onChange={(e) => setGameName(e.target.value)}
                    />
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => CreateGame(gameName)}>Create</Button>
                </DialogActions>
            </Dialog>
        </>
    )
}

export default GameCreator
