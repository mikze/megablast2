import {createListenerMiddleware, createSlice, PayloadAction } from '@reduxjs/toolkit'

interface PlayerStats {
    lives : number,
    bombs : number,
    range : number,
    speed : number
}

const initialState: PlayerStats =
    {
        lives : 0,
        bombs : 0,
        range : 0,
        speed : 0
    };

const playerStatsSlice = createSlice({
    name: 'playerStats',
    initialState,
    reducers: {
        setPlayerStats: (state, action: PayloadAction<PlayerStats>) => {
            state.lives = action.payload.lives;
            state.bombs = action.payload.bombs;
            state.range = action.payload.range;
            state.speed = action.payload.speed;
        }
    }
})

export const { setPlayerStats } = playerStatsSlice.actions
export default playerStatsSlice.reducer