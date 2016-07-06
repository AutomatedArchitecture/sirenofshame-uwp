import { Injectable } from '@angular/core';
import { BaseCommand } from './base.command';
import { ServerService } from '../server.service';
import { CiServer } from '../models/ciServer';
import { MyBuildDefinition } from '../models/myBuildDefinition';

@Injectable()
export class GetProjectsCommand extends BaseCommand {
    constructor(protected serverService: ServerService) {
        super(serverService);

        serverService.registerCommand(this);
    }

    get type() {
        return "getProjects";
    }

    public response(data) { }

    public getProjects(ciServer: CiServer): Promise<MyBuildDefinition[]> {
        return new Promise<MyBuildDefinition[]>((resolve, err) => {
            this.response = (result) => {
                if (result.responseCode === 200) {
                    resolve(result.result);
                } else {
                    err(result.result);
                }
            };
            var sendRequest = {
                type: 'getProjects',
                ciServer: ciServer
            }
            this.serverService.send(sendRequest, err);
        });
    }
}