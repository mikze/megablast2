import {createListenerMiddleware, createSlice, PayloadAction } from '@reduxjs/toolkit'

interface Admin {
    isAdmin : boolean,
}

const initialState: Admin =
    {
        isAdmin : false
    };

const adminSlice = createSlice({
    name: 'admin',
    initialState,
    reducers: {
        setAdmin: (state, action: PayloadAction<Admin>) => {
            console.log('setting admin ', action.payload.isAdmin)
            state.isAdmin = action.payload.isAdmin;
        }
    }
})

export const { setAdmin } = adminSlice.actions
export default adminSlice.reducer