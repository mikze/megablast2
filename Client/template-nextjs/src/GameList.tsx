import { BrowserRouter, Routes, Route } from "react-router";
import { Connection } from "./game/SignalR/Connection";
import { useAppSelector } from "./hooks";
import { Provider } from "react-redux";
import store from "./storesAndReducers/Store";


function GameList() {
    const GetGames = () =>
        Connection.InvokeConnection("GetRunningAllGames");
    
    const count = useAppSelector((state) => state.games.games)
    return (
        <Provider store={store}>
            <>mian
                <>{count.map(g => <div>{g}</div>)}</>
                <button className="button" onClick={() => GetGames()}>Get games!</button>
            </>
        </Provider>
    )
}

export default GameList
