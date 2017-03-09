import {Component} from "@angular/core";
import { DeleteSettingsCommand } from "../commands/delete-settings.command"
import { ServerService } from "../services/server.service";
import { Router } from "@angular/router";

@Component({
    template: `
<h1>Settings</h1>
<button type="button" class="btn btn-danger" (click)="deleteConfig()">Delete Config</button>
<h2>Logs</h2>
<div>{{logs}}</div>
<app-spinner [isBusy]="isBusy"></app-spinner>
`
})
export class Settings {
    constructor(
        private deleteSettingsCommand: DeleteSettingsCommand,
        private router: Router,
        private serverService: ServerService
    ) {
        this.logs = "loading...";
    }

    public logs: string;
    public isBusy: boolean;

    public ngOnInit() {
        this.isBusy = true;
        setTimeout(() => { this.isBusy = false; }, 3000);
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