import { createSlice, PayloadAction } from '@reduxjs/toolkit'

interface GamesState {
    games: string[];
}

const initialState: GamesState = {
    games: []
}

const gamesSlice = createSlice({
    name: 'games',
    initialState,
    reducers: {
        setGames: (state, action: PayloadAction<GamesState>) => {
            state.games = action.payload.games
        }
    }
})

export const { setGames } = gamesSlice.actions
export default gamesSlice.reducer