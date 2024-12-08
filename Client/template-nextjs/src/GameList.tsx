import { BrowserRouter, Routes, Route } from "react-router";
import { Connection } from "./game/SignalR/Connection";
import { useAppSelector } from "./hooks";
import { Provider } from "react-redux";
import store from "./storesAndReducers/Store";
import router from "next/router";
import  configureStore  from './storesAndReducers/Store'
import { chooseGame } from "./storesAndReducers/chosenGameReducer";

function GameList() {
    const GetGames = () =>
        Connection.InvokeConnection("GetRunningAllGames");
    
    const SetGame = (gameName : string) =>
    { 
        console.log("SetGame", gameName);
        new Promise((r,c) =>{  r(localStorage.setItem("gameName", gameName)); })
        .then(() => router.push('/About'));
    }
    
    const count = useAppSelector((state) => state.games.games)
    return (
        <Provider store={store}>
            <>
                <>{count.map(g => <div>{g}
                    <button className="button" onClick={() => SetGame(g)}>Join</button>
                </div>)}</>
                <button className="button" onClick={() => GetGames()}>Get games!</button>
            </>
        </Provider>
    )
}

export default GameList
