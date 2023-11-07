import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [vue()],
    resolve: {
        alias: [{ find: '@', replacement: '/src' }],
    },
    css: {
        preprocessorOptions: {
            scss: {
                additionalData: `
                @import "./src/assets/css/utils/_vars.scss";
                @import "./src/assets/css/utils/_mixins.scss";
                `
            },
        },
    },
});
