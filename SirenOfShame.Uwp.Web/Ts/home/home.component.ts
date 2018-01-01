import {Component, OnInit } from "@angular/core";
import { GetCiEntryPointSettingsCommand } from "../commands/get-cientrypointsettings.command";
import { CiEntryPointSetting } from "../models/ciEntryPointSetting";
import { Router } from "@angular/router";

@Component({
  template: `
    <div *ngIf="!watchingAnyCiServers">
      <h1>Welcome to Siren of Shame!</h1>
      <p>To get started you'll need to add a server to start watching:</p>
      <button type="submit" (click)="onAddServerClick()">Add Server</button>
    </div>
    <div *ngIf="watchingAnyCiServers">
      <h1>Siren of Shame</h1>    
      <p>You are watching {{ciEntryPointSettings.length}} servers</p>
    </div>
`
})
export class Home implements OnInit {
    public ciEntryPointSettings: CiEntryPointSetting[] = [];
    public watchingAnyCiServers: boolean;

    constructor(
      private getCiEntryPointSettingsCommand: GetCiEntryPointSettingsCommand,
      private router: Router
    ) {

      this.getCiEntryPointSettingsCommand.execute()
        .then(ciEntryPoints => {
          this.watchingAnyCiServers = ciEntryPoints.length > 0;
          this.ciEntryPointSettings = ciEntryPoints;
        });
    }

    public connectionStatus: string;

    public onAddServerClick() {
      this.router.navigate(["server"]);
    }

    public ngOnInit() {
    }
}
