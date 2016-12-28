import { Injectable } from '@angular/core';
import { BaseCommand } from '../commands/base.command';
import { ServerService } from '../server.service';
import { BuildStatus } from "../models/buildStatus";

@Injectable()
export class UpdateMockBuildCommand extends BaseCommand
{
    constructor(protected serverService: ServerService) {
        super(serverService);

        serverService.registerCommand(this);
    }

    get type() {
        return "updateMockBuild";
    }

    public response(data) { }

    public execute(buildStatus: BuildStatus): Promise<boolean> {
        return new Promise<boolean>((resolve, err) => {
            this.response = (message) => resolve(message.result);
            var sendRequest = {
                type: this.type,
                message: buildStatus
            };
            this.serverService.send(sendRequest, err);
        });
    }

}