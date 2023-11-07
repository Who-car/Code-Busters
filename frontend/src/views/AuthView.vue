<script setup lang="ts">
import { ref } from 'vue'
import LoginForm from '../components/Forms/LoginForm.vue'
import RegForm from '../components/Forms/RegForm.vue'

const isAuthorization = ref(true)
const isRegistration = ref(false)

const handleSwitchLoginForm = () => isAuthorization.value = !isAuthorization.value
const handleSwitchRegForm = () => isRegistration.value = !isRegistration.value

const onAfterAuthLeave = () => (isRegistration.value = !isRegistration.value)
const onAfterRegLeave = () => (isAuthorization.value = !isAuthorization.value)

</script>

<template>
    <div class="tw-flex tw-items-center tw-max-w-5xl tw-px-4 tw-mx-auto tw-w-full">
        <div
            class="tw-flex tw-justify-center md:tw-justify-between tw-items-center tw-w-full tw-gap-9"
        >
            <img
                src="@/assets/images/auth-image.gif"
                alt=""
                class="tw-max-w-lg tw-object-contain md:tw-block tw-hidden"
            />
            <Transition @after-leave="onAfterAuthLeave">
                <LoginForm
                    v-if="isAuthorization"
                    @switchLoginForm="handleSwitchLoginForm"
                />
            </Transition>
            <Transition @after-leave="onAfterRegLeave">
                <RegForm
                    v-if="isRegistration"
                    @switchRegForm="handleSwitchRegForm"/>
            </Transition>
        </div>
    </div>
</template>
