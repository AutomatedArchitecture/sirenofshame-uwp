import { Injectable } from "@angular/core";
import { BaseCommand } from "./base.command";
import { ServerService } from "../services/server.service";

@Injectable()
export class DeleteServerCommand extends BaseCommand
{
    constructor(protected serverService: ServerService) {
        super(serverService);

        serverService.registerCommand(this);
    }

    get type() {
        return "delete-server";
    }

    public response(data) { }

    public execute(serverId: number): Promise<boolean> {
        return new Promise<boolean>((resolve, err) => {
            this.response = (message) => resolve(message.result);
            var sendRequest = {
                type: this.type,
                message: serverId
            };
            this.serverService.send(sendRequest, err);
        });
    }

}