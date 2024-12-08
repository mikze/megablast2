import {createListenerMiddleware, createSlice, PayloadAction } from '@reduxjs/toolkit'

interface ChosenGame {
    chosenGame : string
}

const initialState: ChosenGame =
    {
        chosenGame : ""
    };

const chooseGameSlice = createSlice({
    name: 'chosenGame',
    initialState,
    reducers: {
        chooseGame: (state, action: PayloadAction<ChosenGame>) => {
            state.chosenGame = action.payload.chosenGame;
        }
    }
})

export const { chooseGame } = chooseGameSlice.actions
export default chooseGameSlice.reducer