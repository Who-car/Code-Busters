<script setup lang="ts">
import { defineProps, ref } from 'vue';

interface Props {
    card: {
        img: string;
        title: string;
        description: string;
        topic: string;
    };
}

const props = defineProps<Props>();
const emit = defineEmits()

const emitSelectedСard = () => {
    emit('selectedСard', props.card.topic)
};
</script>

<template>
    <Card class="tw-w-full" @click="emitSelectedСard()">
        <template #header>
            <img
                :src="card.img"
                class="tw-h-56 tw-w-full tw-object-cover tw-p-3"
            />
        </template>
        <template #title>
            <Chip label="Xuxue Feng" :image="card.img" />
            <div>{{ card.title }}</div>
        </template>
    </Card>
</template>
