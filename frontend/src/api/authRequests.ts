import { config } from '@/config'

export const loginRequest = async (credentials: any) => {
    const result = await fetch(`${config.API_URL}/login`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        body: JSON.stringify(credentials)
    })
    const data = await result.json()
    return data
}

export const regRequest = async (credentials: any) => {
    const result = await fetch(`${config.API_URL}/register`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        body: JSON.stringify(credentials)
    })
    const data = await result.json()
    return data
}

export const getUserRequest = async () => {
    const result = await fetch(
        `${config.API_URL}/v2/Auth/user?id=51d9c82a-d721-4b35-9bb8-8b2dcda6d2dd`,
        {
            method: 'GET',
            headers: {
                'x-api-key': config.TOKEN,
                'Content-Type': 'application/json',
                Authorization: `Bearer eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjUxZDljODJhLWQ3MjEtNGIzNS05YmI4LThiMmRjZGE2ZDJkZCIsInN1YiI6Ik9jaGt5MTIzMjFAZ21haWwuY29tIiwiZW1haWwiOiJPY2hreTEyMzIxQGdtYWlsLmNvbSIsImp0aSI6IjAzODhkYzllLTZmZjUtNGJhNC04ZjY2LTBlOTQyNjE5NjZiNCIsIm5iZiI6MTY5ODY2OTIzNiwiZXhwIjoxNjk4NjgzNjM2LCJpYXQiOjE2OTg2NjkyMzZ9.uuRzGAD3yydXzDP5jUK9JFsKmTa5OobyydOCAt05r0VkjcltZ0rwC-zLMxhDQr57DAXrp_IOt5HneeN5kC55og`
            }
        }
    )
    const data = await result.json()
    return data
}
