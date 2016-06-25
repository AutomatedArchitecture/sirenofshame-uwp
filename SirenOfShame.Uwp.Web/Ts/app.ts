import {Component} from '@angular/core';
import {ROUTER_DIRECTIVES} from '@angular/router';
import {ShowOff} from './show-off.component'
import {Home} from './home.component'
import { ServerService } from './server.service';

@Component({
    selector: 'my-app',
    templateUrl: 'components/app.html',
    providers: [ServerService],
    directives: [ROUTER_DIRECTIVES]
})
export class AppComponent {
    constructor(private serverService: ServerService) {
        serverService.deviceConnectionChanged.subscribe(connected => this.isDeviceConnected = connected);
    }

    public isDeviceConnected: boolean;
}