import { Injectable } from '@angular/core';
import { BaseCommand } from './base.command';
import { ServerService } from '../server.service';
import { CiEntryPointSetting } from '../models/ciEntryPointSetting';

@Injectable()
export class GetCiEntryPointSettingsCommand extends BaseCommand {
    constructor(protected serverService: ServerService) {
        super(serverService);

        serverService.registerCommand(this);
    }

    get type() {
        return "getCiEntryPointSettings";
    }

    public response(data) { }

    public execute(): Promise<CiEntryPointSetting[]> {
        return new Promise<CiEntryPointSetting[]>((resolve, err) => {
            this.response = (result) => {
                if (result.responseCode === 200) {
                    resolve(result.result);
                } else {
                    err(result.result);
                }
            };
            var sendRequest = {
                type: this.type
            }
            this.serverService.send(sendRequest, err);
        });
    }
}