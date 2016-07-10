import {Component} from '@angular/core';
import {ROUTER_DIRECTIVES} from '@angular/router';
import {ShowOff} from './show-off.component'
import {Home} from './home.component'
import { ServerService } from './server.service';
import { EchoCommand } from './commands/echo.command';
import { GetBuildDefinitionsCommand } from './commands/get-builddefinitions.command';
import { GetSirenInfoCommand } from './commands/get-siren-info.command';
import { GetCiEntryPointSettingsCommand } from './commands/get-cientrypointsettings.command';
import { GetCiEntryPointSettingCommand } from './commands/get-cientrypointsetting.command';
import { CiEntryPointSetting } from './models/ciEntryPointSetting';

@Component({
    selector: 'my-app',
    templateUrl: 'components/app.html',
    providers: [ServerService, EchoCommand, GetBuildDefinitionsCommand, GetSirenInfoCommand, GetCiEntryPointSettingsCommand, GetCiEntryPointSettingCommand ],
    directives: [ROUTER_DIRECTIVES]
})
export class AppComponent {
    constructor(private serverService: ServerService, private getCiEntryPointSettingsCommand: GetCiEntryPointSettingsCommand) {
        serverService.connected.subscribe(() => this.onServerConnected());
        serverService.connectionError.subscribe(err => {
            this.webSocketsConnecting = false;
            this.webSocketsError = err;
        });
        serverService.deviceConnectionChanged.subscribe(connected => this.isDeviceConnected = connected);
    }

    private onServerConnected() {
        this.webSocketsConnecting = false;
        this.webSocketsError = null;
        this.getCiEntryPointSettingsCommand.execute()
            .then(ciEntryPoints => this.ciEntryPointSettings = ciEntryPoints);
    }

    public webSocketsConnecting: boolean = true;
    public webSocketsError: string = null;
    public isDeviceConnected: boolean = null;
    public ciEntryPointSettings: CiEntryPointSetting[];
}