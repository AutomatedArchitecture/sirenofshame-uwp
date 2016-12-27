import { Injectable } from '@angular/core';
import { BaseCommand } from './base.command';
import { ServerService } from '../server.service';
import { CiEntryPoint } from '../models/ciEntryPoint';

@Injectable()
export class GetCiEntryPointsCommand extends BaseCommand {
    constructor(protected serverService: ServerService) {
        super(serverService);

        serverService.registerCommand(this);
    }

    get type() {
        return "getCiEntryPoints";
    }

    public response(data) { }

    public execute(): Promise<CiEntryPoint[]> {
        return new Promise<CiEntryPoint[]>((resolve, err) => {
            this.response = (result) => {
                if (result.responseCode === 200) {
                    resolve(result.result);
                } else {
                    err(result.result);
                }
            };
            var sendRequest = {
                type: 'getCiEntryPoints'
            }
            this.serverService.send(sendRequest, err);
        });
    }
}