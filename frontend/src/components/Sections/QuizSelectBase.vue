<template>
    <Dialog
        modal
        v-model:visible="cardDialogVisible"
        :closable="true"
        :draggable="false"
        :style="{
            'max-width': '496px',
            width: '100%',
        }"
    >
        <template #container>
            <Card
                class="tw-w-full tw-h-full"
                @click="cardDialogVisible = false"
            >
                <template #header>
                    <div class="tw-relative">
                        <img
                            src="/src/assets/images/DevOpsLogo.png"
                            class="tw-h-56 tw-w-full tw-object-cover tw-p-3"
                        />
                        <Chip
                            class="tw-absolute tw-bottom-[-4px] tw-left-5"
                            label="Xuxue Feng"
                            image="/src/assets/images/DevOpsLogo.png"
                        />
                    </div>
                </template>
                <template #title>
                    <div>title</div>
                </template>
            </Card>
        </template>
    </Dialog>
    <div class="tw-max-w-6xl tw-mx-auto tw-px-4">
        <div
            id="scrollPoint"
            class="tw-justify-items-center tw-grid tw-grid-cols-1 md:tw-grid-cols-2 lg:tw-grid-cols-3 tw-gap-y-10 tw-gap-x-8 tw-mb-10"
        >
            <QuizCard
                class="tw-transition-transform hover:tw-scale-[1.04]"
                v-for="(card, index) in baseCards"
                :key="index"
                :card="card"
                @selectedСard="handleSelectedСard"
            />
        </div>
    </div>
</template>

<script setup lang="ts">
import QuizCard from '@/components/QuizCard.vue';
import { useRouter } from 'vue-router';
import img_js from '@/assets/images/JavaScriptLogo.png';
import img_html5 from '@/assets/images/HtmlLogo.png';
import img_php from '@/assets/images/PHPLogo.jpg';
import img_devops from '@/assets/images/DevOpsLogo.png';
import img_docker from '@/assets/images/DockerLogo.jpg';
import img_laravel from '@/assets/images/LaravelLogo.jpg';
import img_sql from '@/assets/images/SqlLogo.jpg';
import img_kubernetes from '@/assets/images/KubernetesLogo.png';
import img_linux from '@/assets/images/LinuxLogo.jpg';
import { ref } from 'vue';
const router = useRouter();

const cardDialogVisible = ref(false);

const handleSelectedСard = (data: string) => {
    cardDialogVisible.value = true;

    // router.push({
    //     name: 'Quiz',
    //     params: { topic: data[0] }
    // });
};

const baseCards = [
    {
        img: img_js,
        title: 'JavaScript',
        description:
            'JavaScript Test: unleashing the power of obscure syntax and random bugs',
        topic: 'JavaScript',
    },
    {
        img: img_html5,
        title: 'HTML5',
        description:
            'HTML Test: Can you spot the missing closing tag? Good luck finding Waldo!',
        topic: 'HTML',
    },
    {
        img: img_php,
        title: 'PHP',
        description:
            'CSS Test: Think its just about colors and fonts? Time to dive into the Bermuda Triangle of Cascading Style Sheets!',
        topic: 'PHP',
    },
    {
        img: img_devops,
        title: 'DevOps',
        description:
            'DevOps Test: Are you a master of juggling flaming chainsaws while balancing on a tightrope? This quiz is childs play for you!',
        topic: 'DevOps',
    },
    {
        img: img_docker,
        title: 'Docker',
        description:
            'Docker Test: Can you keep up with the pace of containers appearing and disappearing faster than Houdinis vanishing act?',
        topic: 'Docker',
    },
    {
        img: img_laravel,
        title: 'Laravel',
        description:
            'Laravel Test: Do you have the patience of a Zen master to untangle the labyrinth of Laravels elegant yet mysterious code?',
        topic: 'Laravel',
    },
    {
        img: img_sql,
        title: 'MySQL',
        description:
            'SQL Test: Ready to navigate through the twisted maze of database queries with just your wits and a handful of wildcards?',
        topic: 'MySQL',
    },
    {
        img: img_kubernetes,
        title: 'Kubernetes',
        description:
            'Kubernetes Test: Think you can orchestrate a cluster of virtual minions like a puppet master? Time to prove your Kubernetes command!',
        topic: 'Kubernetes',
    },
    {
        img: img_linux,
        title: 'Linux',
        description:
            'Linux Test: Are you a Linux wizard who can recite every terminal command while blindfolded? Brace yourself for the ultimate Linux showdown!',
        topic: 'Linux',
    },
];
</script>
