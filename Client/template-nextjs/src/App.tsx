import { BrowserRouter, Routes, Route } from "react-router";
import { Connection } from "./game/SignalR/Connection";
import { useState } from "react";
import { useAppSelector } from "./hooks";
import { Provider } from "react-redux";
import store from "./storesAndReducers/Store";
import GameList from "./GameList";


function App() {
    Connection.CreateConnection();
    
    const CreateGame = (gameName : string) =>
        Connection.InvokeConnection("CreateGame", gameName);
    
    const GetGames = () =>
        Connection.InvokeConnection("GetRunningAllGames");

    const [gameName, setGameName] = useState("");
    return (
        <Provider store={store}>
        <>Name of new game
            <input value={gameName} onChange={(e) => setGameName(e.target.value)}/>
            <button className="button" onClick={() => CreateGame(gameName)}>Create game!</button>
            <GameList/>
        </>
        </Provider>
    )
}

export default App
