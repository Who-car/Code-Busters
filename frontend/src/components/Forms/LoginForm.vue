<script setup lang="ts">
import { useForm, useField } from 'vee-validate'
import { ref } from 'vue'
import * as Yup from 'yup'
import { useAuthStore } from '@/stores/authStore'

const { signin } = useAuthStore()

interface LoginForm {
    email: string
    password: string
}

const emit = defineEmits()

const loginSchema: Yup.ObjectSchema<LoginForm> = Yup.object({
    email: Yup.string().required(),
    password: Yup.string().min(6).required()
})

const { handleSubmit } = useForm<LoginForm>({
    validationSchema: loginSchema
})
const { value: email, errors: loginErrors } = useField<LoginForm['email']>('email')
const { value: password, errors: passwordErrors } = useField<LoginForm['password']>('password')

const onSubmit = handleSubmit(async (values) => {
    const data = await signin(values)
})
</script>

<template>
    <form
        @submit="onSubmit"
        class="tw-flex tw-flex-col auth__input-container tw-self-center tw-w-full tw-max-w-sm"
    >
        <div class="tw-flex tw-flex-col tw-self-center tw-w-full tw-max-w-sm tw-gap-2">
            <InputText
                v-model="email"
                size="large"
                name="email"
                autocomplete="login"
                id="login"
                placeholder="Email"
                :class="{ 'p-invalid': loginErrors.length }"
            />
            <InputText
                v-model="password"
                size="large"
                type="password"
                name="password"
                id="authPassword"
                autocomplete="current-password"
                placeholder="Password"
                :class="{ 'p-invalid': passwordErrors.length }"
            />
            <Button type="submit" class="tw-justify-center tw-text-2xl"><span>log in</span></Button>
        </div>
        <div class="tw-flex tw-items-center">
            <div class="tw-h-1 tw-w-full tw-bg-black" />
            <div
                class="tw-font-base-ui tw-text-xl tw-mx-3 tw-text-black tw-relative tw-bottom-[2px]"
            >
                or
            </div>
            <div class="tw-h-1 tw-w-full tw-bg-black" />
        </div>
        <Button
            @click="emit('switchLoginForm')"
            class="tw-justify-center tw-text-2xl tw-mb-3"
            outlined
        >
            <span>sign up</span>
        </Button>
    </form>
</template>
