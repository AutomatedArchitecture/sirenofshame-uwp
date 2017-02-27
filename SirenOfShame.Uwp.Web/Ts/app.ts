import { Component } from '@angular/core';
import { ServerService } from './server.service';
import { EchoCommand } from './commands/echo.command';
import { GetBuildDefinitionsCommand } from './commands/get-builddefinitions.command';
import { GetSirenInfoCommand } from './commands/get-siren-info.command';
import { GetCiEntryPointSettingsCommand } from './commands/get-cientrypointsettings.command';
import { GetCiEntryPointSettingCommand } from './commands/get-cientrypointsetting.command';
import { AddCiEntryPointSettingCommand } from './commands/add-cientrypointsetting.command';
import { CiEntryPointSetting } from './models/ciEntryPointSetting';
import { DeleteSettingsCommand } from './commands/delete-settings.command';
import { DeleteServerCommand } from './commands/delete-server.command';
import { GetCiEntryPointsCommand } from './commands/get-cientrypoints.command';
import { UpdateMockBuildCommand } from './mock/update-mock-build.command'

@Component({
    selector: 'my-app',
    templateUrl: 'components/app.html',
    providers: [
        ServerService, EchoCommand, GetBuildDefinitionsCommand, GetSirenInfoCommand, GetCiEntryPointSettingsCommand,
        GetCiEntryPointSettingCommand, AddCiEntryPointSettingCommand, DeleteSettingsCommand,
        GetCiEntryPointsCommand, UpdateMockBuildCommand, DeleteServerCommand
    ]
})
export class AppComponent {
    constructor(
        private serverService: ServerService,
        private getCiEntryPointSettingsCommand: GetCiEntryPointSettingsCommand
    ) {
        serverService.connected.subscribe(() => this.onServerConnected());
        serverService.serverAdded.subscribe(ciEntryPointSetting => this.onServerAdded(ciEntryPointSetting));
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

    private onServerAdded(ciEntryPointSetting: CiEntryPointSetting) {
        this.ciEntryPointSettings.push(ciEntryPointSetting);
    }

    public webSocketsConnecting: boolean = true;
    public webSocketsError: string = null;
    public isDeviceConnected: boolean = null;
    public ciEntryPointSettings: CiEntryPointSetting[];
}