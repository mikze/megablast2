import { configureStore } from '@reduxjs/toolkit'
import chatReducer from './chatReducer'
import configReducer from './configReducer';
import adminReducer from './adminReducer'
import gamesReducer from './gamesReducer';
import chosenGameReducer from './chosenGameReducer';
import playerStatsReducer from './statsReducer'

const store = configureStore({
    reducer: {
        chat: chatReducer,
        config: configReducer,
        admin: adminReducer,
        games: gamesReducer,
        chosenGame: chosenGameReducer,
        playerStats: playerStatsReducer
    },
})

store.subscribe(() => {console.log('SUBBs', store.getState())});

export default store;
// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<typeof store.getState>
// Inferred type: {posts: PostsState, comments: CommentsState, users: UsersState}
export type AppDispatch = typeof store.dispatch