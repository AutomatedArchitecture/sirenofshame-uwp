import { provideRouter, RouterConfig } from '@angular/router';
import {ShowOff} from './show-off.component'
import {Home} from './home.component'
import {Server} from './server.component'
import {Settings} from './settings.component'

export const routes: RouterConfig = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: Home },
    { path: 'showoff', component: ShowOff },
    { path: 'server', component: Server },
    { path: 'server/:id', component: Server },
    { path: 'settings', component: Settings }
];

export const APP_ROUTER_PROVIDERS = [
    provideRouter(routes)
];