import { Injectable } from '@angular/core';
import { BaseCommand } from './base.command';
import { ServerService } from '../server.service';
import { MyBuildDefinition } from '../models/myBuildDefinition';
import { CiEntryPointSetting } from '../models/ciEntryPointSetting';

@Injectable()
export class GetBuildDefinitionsCommand extends BaseCommand {
    constructor(protected serverService: ServerService) {
        super(serverService);

        serverService.registerCommand(this);
    }

    get type() {
        return "getBuildDefinitions";
    }

    public response(data) { }

    public getBuildDefinitions(ciEntryPointSetting: CiEntryPointSetting): Promise<MyBuildDefinition[]> {
        return new Promise<MyBuildDefinition[]>((resolve, err) => {
            this.response = (result) => {
                if (result.responseCode === 200) {
                    resolve(result.result);
                } else {
                    err(result.result);
                }
            };
            var sendRequest = {
                type: 'getBuildDefinitions',
                ciServer: ciEntryPointSetting
            }
            this.serverService.send(sendRequest, err);
        });
    }
}