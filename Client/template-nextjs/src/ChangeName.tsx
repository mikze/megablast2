import { useState } from "react";

type Props = {
    changeName: Function 
}

function ChangeName(props: Props) {
    // The sprite can only be moved in the MainMenu Scene
    //  References to the PhaserGame component (game and scene are exposed)
    const [name, setName] = useState("");
    
    return(
        <div>
            <input value={name} onChange={(e) => setName(e.target.value)} />
            <button className="button" onClick={() => props.changeName(name)}>Change name</button>
        </div>)
}

export default ChangeName