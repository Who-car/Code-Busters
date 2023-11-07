<template>
    <div class="app tw-bg-base-background tw-relative">
        <TheHeader/>
        <router-view/>
        <TheFooter v-if="!route.path.includes('/quiz')" />
        <QuizParticleSystem class="tw-z-[-10]"/>
    </div>
</template>

<script setup lang="ts">
import TheHeader from '@/components/TheHeader.vue'
import TheFooter from '@/components/TheFooter.vue'
import QuizParticleSystem from './components/QuizParticleSystem.vue';
import { useRoute } from 'vue-router'

const route = useRoute()
</script>

<style scoped>
.app {
    z-index: 10;
    min-height: 100%;
    display: grid;
    grid-template-rows: auto 1fr auto;
}
</style>
