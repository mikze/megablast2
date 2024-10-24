import { useRef, useState } from 'react';
import { IRefPhaserGame, PhaserGame } from './game/PhaserGame';
import { GameLevel } from './game/scenes/GameLevel';
import { TextField } from '@mui/material';
import { Preloader } from './game/scenes/Preloader';
import { Scene } from 'phaser';
import { Lobby } from './game/scenes/Lobby';

function App() {
    // The sprite can only be moved in the MainMenu Scene
    const [canMoveSprite, setCanMoveSprite] = useState(true);
    //  References to the PhaserGame component (game and scene are exposed)
    const phaserRef = useRef<IRefPhaserGame | null>(null);
    const [message, setMessage] = useState<string>("");
    const [name, setName] = useState("");
    const [messages, setMessages] = useState<any>([])
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

    const sendMsg = () => {

        if (phaserRef.current) {
            const scene = phaserRef.current.scene as GameLevel;

            if (scene && scene.scene.key === 'GameLevel') {
                scene.sendMsg("user x", message);
                setMessages([...messages, { msg: message }]);
            }
        }
    }

    const changeName = () => {
        if (phaserRef.current) {
            const scene = phaserRef.current.scene as Lobby;

            if (scene && scene.scene.key === 'GameLevel' || scene.scene.key === 'Lobby') {
                scene.changeName(name);
            }
        }
    }

    // Event emitted from the PhaserGame component
    const currentScene = (scene: Phaser.Scene) => {

        setCanMoveSprite(scene.scene.key !== 'MainMenu');
        if (scene.scene.key === "Lobby") {
            setBackToLobbyIsVisible(false);
            setStartIsVisible(true);
        }
        if (scene.scene.key === "GameLevel") {
            setBackToLobbyIsVisible(true);
            setStartIsVisible(false);
        }

    }

    return (
        <div id="app">
            <div>
                <PhaserGame ref={phaserRef} currentActiveScene={currentScene} />
                <div >
                    <TextField value={message} onChange={(e) => setMessage(e.target.value)} id="fullWidth" label="Filled" variant="filled" />
                    <button className="button" onClick={sendMsg}>Send msg</button>
                </div>
                {messages.map((m: { msg: any; }) => m.msg)}
            </div>
            <div>

                {isVisibleStart && (<div>
                    <button className="button" onClick={changeScene}>Start</button>
                </div>)}
                {isVisibleBackToLobby && (
                    <div>
                        <button className="button" onClick={backToLobby}>Back to lobby</button>
                    </div>
                )}
                <div>
                    <input value={name} onChange={(e) => setName(e.target.value)} />
                    <button className="button" onClick={changeName}>Change name</button>
                </div>

            </div>
        </div>
    )
}

export default App
