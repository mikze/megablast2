import { createSlice, PayloadAction } from '@reduxjs/toolkit'

interface Message {
    username: string,
    message: string
}
interface MessagesState {
    messages: Message[];
}

const initialState: MessagesState = {
    messages: []
}

const messageSlice = createSlice({
    name: 'message',
    initialState,
    reducers: {
        receiveMessage: (state, action: PayloadAction<Message>) => {
            state.messages = [...state.messages, action.payload]
        }
    }
})

export const { receiveMessage } = messageSlice.actions
export default messageSlice.reducer