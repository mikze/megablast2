import { BrowserRouter, Routes, Route } from "react-router";
import { Connection } from "./game/SignalR/Connection";
import { useState } from "react";
import { useAppSelector } from "./hooks";
import { Provider } from "react-redux";
import store from "./storesAndReducers/Store";
import GameList from "./GameList";
import GameCreator from "./GameCreator";


function App() {
    Connection.CreateConnection();

    const [showCreator, setShowCreator] = useState(false);
    return (
        <Provider store={store}>
            <>
                <button className="button" onClick={() => setShowCreator(!showCreator)}>Ceate new game</button>
                {showCreator && <GameCreator/>}
                <GameList/>
            </>
        </Provider>
    )
}

export default App
