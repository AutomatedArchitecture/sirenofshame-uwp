import {Component} from '@angular/core';
import { DeleteSettingsCommand } from './commands/delete-settings.command'

@Component({
    template: `
<h1>Settings</h1>
<button type="button" class="btn btn-danger" (click)="deleteConfig()">Delete Config</button>
`
})
export class Settings {
    constructor(private deleteSettingsCommand: DeleteSettingsCommand) {  }

    public deleteConfig() {
        if (confirm('Are you 100% sure?')) {
            this.deleteSettingsCommand.execute()
                .then(() => {
                    alert('settings deleted');
                });
        };
    }
}