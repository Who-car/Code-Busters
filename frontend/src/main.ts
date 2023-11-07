import App from '@/App.vue'
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import router from '@/router/router.js'
import '@/assets/css/index.scss'
import PrimeVue from 'primevue/config';
import 'primeicons/primeicons.css'

//primevue components
import Button from 'primevue/button'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import Avatar from 'primevue/avatar'
import InputNumber from 'primevue/inputnumber'
import Card from 'primevue/card'
import Chip from 'primevue/chip'
import InlineMessage from 'primevue/inlinemessage'
import Toast from 'primevue/toast';
import ToastService from 'primevue/toastservice';


const pinia = createPinia()
const app = createApp(App)

app.component('Dialog', Dialog)
app.component('Button', Button)
app.component('InputText', InputText)
app.component('Avatar', Avatar)
app.component('InputNumber', InputNumber)
app.component('Card', Card)
app.component('Chip', Chip)
app.component('InlineMessage', InlineMessage)
app.component('Toast', Toast)

app.use(pinia)
app.use(router)
app.use(PrimeVue);
app.use(ToastService);
app.mount('#app')