import { BrowserRouter, Routes, Route } from "react-router";
import { Connection } from "./game/SignalR/Connection";
import { useAppSelector } from "./hooks";
import { Provider } from "react-redux";
import store from "./storesAndReducers/Store";
import router from "next/router";
import  configureStore  from './storesAndReducers/Store'
import { chooseGame } from "./storesAndReducers/chosenGameReducer";
import React from "react";
import {Button, Grid, List, ListItem, ListItemText, Paper, Table,
    TableBody,
    TableCell, TableContainer, TableHead, TableRow, Typography, styled } from "@mui/material";

const SetGame = (gameName : string) =>
{
    console.log("SetGame", gameName);
    new Promise((r,c) =>{  r(localStorage.setItem("gameName", gameName)); })
        .then(() => router.push('/RunGame'));
}

const Demo = styled('div')(({ theme }) => 
    ({
}));

function GameList() {
    const GetGames = () =>
        Connection.InvokeConnection("GetRunningAllGames");
    
    
    const count = useAppSelector((state) => state.games.games)
    const [secondary, setSecondary] = React.useState(true);
    
    return (
        <Provider store={store}>
            <>
                <button className="button" onClick={() => GetGames()}>Get games!</button>

                <Grid item xs={12} md={6} width="20%">
                    <Typography sx={{ mt: 4, mb: 2 }} variant="h6" component="div">
                        Game list:
                    </Typography>
                    <Demo>
                        <List dense={false}>
                            {count.map( g =>
                                <ListItem key={g} >
                                    <ListItemText
                                        primary={g}
                                        secondary={secondary ? 'Secondary text' : null}
                                    />           
                                              
                                    <Button variant="contained" endIcon="=>" onClick={() => SetGame(g)}>
                                        Join
                                    </Button>
                                </ListItem>
                            )}
                        </List>
                    </Demo>
                </Grid>
                
            </>
        </Provider>
    )
}

export default GameList
