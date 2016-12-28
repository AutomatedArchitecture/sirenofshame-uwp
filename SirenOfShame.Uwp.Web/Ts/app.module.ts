import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule }    from '@angular/forms';
import { AppComponent }  from './app';
import { AppRoutingModule }     from './app.routes';
import {Home} from './home.component'
import {ShowOff} from './show-off.component'
import {Server} from './server.component'
import {Settings} from './settings.component'
import {MockServer} from './mock/mock-server.component'
import {MockBuild} from './mock/mock-build.component'

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
        Settings,
        MockServer,
        MockBuild
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
