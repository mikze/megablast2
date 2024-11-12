import { configureStore } from '@reduxjs/toolkit'
import chatReducer from './chatReciever'

const store = configureStore({
    reducer: {
        chat: chatReducer,
    },
})
export default store;
// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<typeof store.getState>
// Inferred type: {posts: PostsState, comments: CommentsState, users: UsersState}
export type AppDispatch = typeof store.dispatch