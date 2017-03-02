import { NgModule }      from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { FormsModule }    from "@angular/forms";
import { AppComponent }  from "./app";
import { AppRoutingModule }     from "./app.routes";
import {Home} from "./home/home.component";
import {ShowOff} from "./showOff/show-off.component";
import {Server} from "./ciServers/server.component";
import {AddBuild} from "./ciServers/addbuild.component";
import {Settings} from "./settings/settings.component";
import {MockServer} from "./mock/mock-server.component";
import {MockBuild} from "./mock/mock-build.component";
import {ModalComponent} from "./common/modal-component";

@NgModule({
    imports: [
        BrowserModule,
        AppRoutingModule,
        FormsModule
    ],
    declarations: [
        AppComponent,
        Home,
        ShowOff,
        Server,
        AddBuild,
        Settings,
        MockServer,
        MockBuild,
        ModalComponent
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
