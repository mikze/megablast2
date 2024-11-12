import { useRef, useState } from 'react';
import { IRefPhaserGame, PhaserGame } from './game/PhaserGame';
import { GameLevel } from './game/scenes/GameLevel';
import { Preloader } from './game/scenes/Preloader';
import { Lobby } from './game/scenes/Lobby';
import store from './Store'
import { Provider } from 'react-redux'
import ChangeName from './ChangeName';
import Chat from './Chat';
import output from '../public/assets/output.jpg';
import output2 from '../public/assets/output2.jpg';

function App() {
    //  References to the PhaserGame component (game and scene are exposed)
    const phaserRef = useRef<IRefPhaserGame | null>(null);
    const [isVisibleBackToLobby, setBackToLobbyIsVisible] = useState(true);
    const [isVisibleStart, setStartIsVisible] = useState(true);

    const changeScene = () => {

        if (phaserRef.current) {
            let scene = phaserRef.current.scene as Preloader;
            scene.changeScene();
        }
    }

    const backToLobby = () => {

        if (phaserRef.current) {
            let scene = phaserRef.current.scene as GameLevel;
            scene.backToLobby();
        }
    }

    const sendMsg = (message: string) => {

        if (phaserRef.current) {
            const scene = phaserRef.current.scene as GameLevel;

            if (scene && scene.scene.key === 'GameLevel' || scene.scene.key === 'Lobby') {
                scene.sendMsg("user x", message);
            }
        }
    }

    const changeName = (name: string) => {
        console.log("change name");
        if (phaserRef.current) {
            const scene = phaserRef.current.scene as Lobby;

            if (scene && scene.scene.key === 'GameLevel' || scene.scene.key === 'Lobby') {
                scene.changeName(name);
            }
        }
    }
    
    const currentScene = (scene: Phaser.Scene) => {
        if (scene.scene.key === "Lobby") {
            setBackToLobbyIsVisible(false);
            setStartIsVisible(true);
        }
        if (scene.scene.key === "GameLevel") {
            setBackToLobbyIsVisible(true);
            setStartIsVisible(false);
        }
    }

    const ButtonGroup = () => (
        <div>
            
            <div>
                {!isVisibleBackToLobby && <ChangeName changeName={changeName}/>}
            </div>
        </div>
    );

    return (
        <Provider store={store}>
            <div id="app">
                <div id="borderimg">
                    <div className="container">
                        <div className="item2">
                            <ButtonGroup/>
                        </div>
                        <div>
                            <PhaserGame ref={phaserRef} currentActiveScene={currentScene}/>
                        </div>
                        <div>
                            <Chat sendMsg={sendMsg}/>
                        </div>
                        {isVisibleBackToLobby && (
                            <div>
                                <button className="button" onClick={backToLobby}>Back to lobby</button>
                            </div>
                        )}
                        {isVisibleStart && (<div>
                            <button className="button" onClick={changeScene}>Start</button>
                        </div>)}
                    </div>

                </div>

            </div>
        </Provider>
    )
}

export default App
