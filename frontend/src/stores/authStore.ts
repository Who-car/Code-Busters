import { defineStore } from 'pinia'
import { loginRequest, regRequest, getUserRequest } from '@/api/authRequests'
import { ref } from 'vue'

export const useAuthStore = defineStore('authStore', () => {
    const errors = ref<string[]>([])

    const signin = async (credentials: any) => {
        const data = await loginRequest(credentials)
        return data
    }
    const signup = async (credentials: any) => {
        const data = await regRequest(credentials)
        return data
    }
    const getUserInfo = async () => {
        const data = await getUserRequest()
        return data
    }

    return { signin, signup, getUserInfo }
})
