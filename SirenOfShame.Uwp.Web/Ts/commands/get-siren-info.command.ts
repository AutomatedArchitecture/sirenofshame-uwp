﻿import { Injectable } from '@angular/core';
import { BaseCommand } from './base.command';
import { ServerService } from '../server.service';

@Injectable()
export class GetSirenInfoCommand extends BaseCommand {
    constructor(protected serverService: ServerService) {
        super(serverService);

        serverService.registerCommand(this);
    }

    get type() {
        return "getSirenInfo";
    }

    public response(data) { }

    public getSirenInfo(): Promise<ISirenInfo> {
        return new Promise<ISirenInfo>((resolve, err) => {
            this.response = (sirenInfo) => resolve(sirenInfo.result);
            var sendRequest = {
                type: 'getSirenInfo'
            }
            this.serverService.send(sendRequest, err);
        }
        );
    }

    public playLedPattern(id: number) {
        var sendRequest = {
            type: 'playLedPattern',
            id: id
        }
        this.serverService.send(sendRequest);
    }

    public playAudioPattern(id: number) {
        var sendRequest = {
            type: 'playAudioPattern',
            id: id
        }
        this.serverService.send(sendRequest);
    }
}