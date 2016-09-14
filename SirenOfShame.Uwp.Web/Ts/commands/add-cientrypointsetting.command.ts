import { Injectable } from '@angular/core';
import { BaseCommand } from './base.command';
import { ServerService } from '../server.service';
import { CiEntryPointSetting } from '../models/ciEntryPointSetting';

@Injectable()
export class AddCiEntryPointSettingCommand extends BaseCommand {
    constructor(protected serverService: ServerService) {
        super(serverService);

        serverService.registerCommand(this);
    }

    get type() {
        return "addCiEntryPointSetting";
    }

    public response(data) { }

    public invoke(ciEntryPointSetting: CiEntryPointSetting): Promise<number> {
        return new Promise<number>((resolve, err) => {
            this.response = (result) => {
                if (result.responseCode === 200) {
                    resolve(result.result);
                } else {
                    err(result.result);
                }
            };
            var sendRequest = {
                type: 'addCiEntryPointSetting',
                ciEntryPointSetting: ciEntryPointSetting
            }
            this.serverService.send(sendRequest, err);
        });
    }
}