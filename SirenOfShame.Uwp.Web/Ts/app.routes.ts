import { NgModule }             from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {ShowOff} from './show-off.component'
import {Home} from './home.component'
import {Server} from './server.component'
import {Settings} from './settings.component'

const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: Home },
    { path: 'showoff', component: ShowOff },
    { path: 'server', component: Server },
    { path: 'server/:id', component: Server },
    { path: 'settings', component: Settings }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
