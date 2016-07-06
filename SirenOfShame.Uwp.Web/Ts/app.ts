import {Component} from '@angular/core';
import {ROUTER_DIRECTIVES} from '@angular/router';
import {ShowOff} from './show-off.component'
import {Home} from './home.component'
import { ServerService } from './server.service';
import { EchoCommand } from './commands/echo.command';
import { GetProjectsCommand } from './commands/get-projects.command';
import { GetSirenInfoCommand } from './commands/get-siren-info.command';

@Component({
    selector: 'my-app',
    templateUrl: 'components/app.html',
    providers: [ServerService, EchoCommand, GetProjectsCommand, GetSirenInfoCommand ],
    directives: [ROUTER_DIRECTIVES]
})
export class AppComponent {
    constructor(private serverService: ServerService) {
        serverService.connected.subscribe(() => {
            this.webSocketsConnecting = false;
            this.webSocketsError = null;
        });
        serverService.connectionError.subscribe(err => this.webSocketsError = err);
        serverService.deviceConnectionChanged.subscribe(connected => this.isDeviceConnected = connected);
    }

    public webSocketsConnecting: boolean = true;
    public webSocketsError: string = null;
    public isDeviceConnected: boolean = null;
}