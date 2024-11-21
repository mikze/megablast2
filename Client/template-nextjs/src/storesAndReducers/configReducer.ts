import { createSlice, PayloadAction } from '@reduxjs/toolkit'

interface Config {
      monsterAmount : number,
      monsterSpeed: number
      bombDelay: number
}

const initialState = {
    monsterAmount: 0,
    monsterSpeed: 0,
    bombDelay: 0
}

const configSlice = createSlice({
    name: 'config',
    initialState,
    reducers: {
        updateConfig: (state, action: PayloadAction<Config>) => {
            console.log('Payload ', action.payload);
            state.bombDelay = action.payload.bombDelay;
            state.monsterAmount = action.payload.monsterAmount;
            state.monsterSpeed = action.payload.monsterSpeed;
        }
    }
})

export const { updateConfig } = configSlice.actions
export default configSlice.reducer