import { Injectable } from '@angular/core';
import { BaseCommand } from './base.command';
import { ServerService } from '../services/server.service';

@Injectable()
export class EchoCommand extends BaseCommand
{
    constructor(protected serverService: ServerService) {
        super(serverService);

        serverService.registerCommand(this);
    }

    get type() {
        return "echo";
    }

    public response(data) { }

    public echo(message: string): Promise<string> {
        return new Promise<string>((resolve, err) => {
            this.response = (message) => resolve(message.result);
            var sendRequest = {
                type: this.type,
                message: message
            };
            this.serverService.send(sendRequest, err);
        });
    }

}