import { NgModule }             from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import {ShowOff} from "./showOff/show-off.component"
import {Home} from "./home/home.component"
import {Server} from "./ciServers/server.component"
import {Settings} from "./settings/settings.component"
import {MockServer} from "./mock/mock-server.component"

const routes: Routes = [
    { path: "", redirectTo: "home", pathMatch: "full" },
    { path: "home", component: Home },
    { path: "showoff", component: ShowOff },
    { path: "server", component: Server },
    { path: "server/:id", component: Server },
    { path: "settings", component: Settings },
    { path: "mockServer", component: MockServer }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
