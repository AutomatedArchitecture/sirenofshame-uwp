import { provideRouter, RouterConfig } from '@angular/router';
import {ShowOff} from './show-off.component'
import {Home} from './home.component'

export const routes: RouterConfig = [
    { path: '', component: Home },
    { path: 'showoff', component: ShowOff }
];

export const APP_ROUTER_PROVIDERS = [
    provideRouter(routes)
];