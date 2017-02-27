import { Injectable } from '@angular/core';
import { BaseCommand } from './base.command';
import { ServerService } from '../services/server.service';

@Injectable()
export class DeleteSettingsCommand extends BaseCommand
{
    constructor(protected serverService: ServerService) {
        super(serverService);

        serverService.registerCommand(this);
    }

    get type() {
        return "delete-settings";
    }

    public response(data) { }

    public execute(): Promise<boolean> {
        return new Promise<boolean>((resolve, err) => {
            this.response = (message) => resolve(message.result);
            var sendRequest = {
                type: this.type
            };
            this.serverService.send(sendRequest, err);
        });
    }

}