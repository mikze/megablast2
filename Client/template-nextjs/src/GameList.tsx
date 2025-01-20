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
    TableCell, TableContainer, TableHead, TableRow, Typography, styled,
    tableCellClasses} from "@mui/material";

const StyledTableCell = styled(TableCell)(({ theme }) => ({
    [`&.${tableCellClasses.head}`]: {
        backgroundColor: theme.palette.common.black,
        color: theme.palette.common.white,
    },
    [`&.${tableCellClasses.body}`]: {
        fontSize: 14,
    },
}));

const StyledTableRow = styled(TableRow)(({ theme }) => ({
    '&:nth-of-type(odd)': {
        backgroundColor: theme.palette.action.hover,
    },
    // hide last border
    '&:last-child td, &:last-child th': {
        border: 0,
    },
}));

interface GameInfo {
    name: string,
    activePlayers : number,
    maxPlayers: number,
    passRequired: boolean
}

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
    
    return (
        <Provider store={store}>
            <>
                <button className="button" onClick={() => GetGames()}>Refresh game list</button>
                <TableContainer component={Paper}>
                    <Table sx={{ minWidth: 700 }} aria-label="customized table">
                        <TableHead>
                            <TableRow>
                                <StyledTableCell>Game name</StyledTableCell>
                                <StyledTableCell align="right">Active players</StyledTableCell>
                                <StyledTableCell align="right">Max players</StyledTableCell>
                                <StyledTableCell align="right">Password</StyledTableCell>
                                <StyledTableCell align="right"></StyledTableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {count.map((row) => (
                                <StyledTableRow key={row.name}>
                                    <StyledTableCell component="th" scope="row">
                                        {row.name}
                                    </StyledTableCell>
                                    <StyledTableCell align="right">{row.activePlayers}</StyledTableCell>
                                    <StyledTableCell align="right">{row.maxPlayers}</StyledTableCell>
                                    <StyledTableCell align="right">{row.passRequired ? 'YES' : 'NO'}</StyledTableCell>
                                    <StyledTableCell align="right">
                                    <Button variant="contained" onClick={() => SetGame(row.name)}>
                                        Join
                                    </Button>
                                    </StyledTableCell>
                                    
                                </StyledTableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            </>
        </Provider>
    )
}

export default GameList
