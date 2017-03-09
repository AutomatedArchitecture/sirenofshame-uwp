import { Injectable } from "@angular/core";
import { BaseCommand } from "./base.command";
import { ServerService } from "../services/server.service";

@Injectable()
export class GetLogsCommand extends BaseCommand
{
    constructor(protected serverService: ServerService) {
        super(serverService);

        serverService.registerCommand(this);
    }

    get type() {
        return "get-logs";
    }

    public response(data) { }

    public execute(): Promise<string[]> {
        return new Promise<string[]>((resolve, err) => {
            this.response = (result) => {
                if (result.responseCode === 200) {
                    resolve(result.result);
                } else {
                    err(result.result);
                }
            };
            var sendRequest = {
                type: this.type
            };
            this.serverService.send(sendRequest, err);
        });
    }

}