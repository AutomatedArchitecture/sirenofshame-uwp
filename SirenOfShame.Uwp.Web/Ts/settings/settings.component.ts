import {Component} from "@angular/core";
import { DeleteSettingsCommand } from "../commands/delete-settings.command"
import { GetLogsCommand } from "../commands/get-logs.command"
import { ServerService } from "../services/server.service";
import { Router } from "@angular/router";

@Component({
    template: `
<h1>Settings</h1>
<button type="button" class="btn btn-danger" (click)="deleteConfig()">Delete Config</button>
<h2>Logs</h2>
<button type="button" (click)="getLogs()" class="btn btn-default">Get Logs</button>
<pre class="pre-scrollable" *ngIf="logs">{{logs}}</pre>
<app-spinner [isBusy]="isBusy"></app-spinner>
`
})
export class Settings {
    constructor(
        private deleteSettingsCommand: DeleteSettingsCommand,
        private router: Router,
        private serverService: ServerService,
        private getLogsCommand: GetLogsCommand
    ) {
        this.logs = null;
    }

    public logs: string;
    public isBusy: boolean = false;

    public getLogs() {
        this.isBusy = true;

        this.getLogsCommand.execute()
            .then((logs: string[]) => {
                this.isBusy = false;
                this.logs = logs.join("\n");
            });

    }

    public deleteConfig() {
        if (confirm("Are you 100% sure?")) {
            this.deleteSettingsCommand.execute()
                .then(() => {
                    alert("settings deleted");
                    this.serverService.refreshCiEntryPoints.emit();
                    this.router.navigate(["home"]);
                });
        };
    }
}