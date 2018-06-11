import { Injectable } from "@angular/core";
import { BaseCommand } from "./base.command";
import { ServerService } from "../services/server.service";
import { CiEntryPointSetting } from "../models/ciEntryPointSetting";

@Injectable()
export class GetCiEntryPointSettingsCommand extends BaseCommand {
  constructor(protected serverService: ServerService) {
    super(serverService);

    serverService.registerCommand(this);
  }

  get type() {
    return "getCiEntryPointSettings";
  }

  private activePromises: ActivePromise[] = [];

  public response(data) {
    var promise = this.activePromises.pop();
    if (data.responseCode === 200) {
      promise.resolve(data.result);
    } else {
      promise.err(data.result);
    }
  }

  public execute(): Promise<CiEntryPointSetting[]> {
    var promise = new Promise<CiEntryPointSetting[]>((resolve, err) => {
      this.activePromises.push({ resolve: resolve, err: err });
      var sendRequest = {
        type: this.type
      };
      this.serverService.send(sendRequest, err);
    });
    return promise;
  }
}

interface ActivePromise {
  resolve;
  err;
}