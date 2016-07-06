import {Component} from '@angular/core';
import {ROUTER_DIRECTIVES} from '@angular/router';
import 'rxjs/add/operator/toPromise';
import { GetSirenInfoCommand } from './commands/get-siren-info.command';

@Component({
    template: `
<h1>Show Off</h1>
<form class="form-horizontal">
    <div class="form-group">
        <label for="ledPattern" class="col-sm-2 control-label">LED Pattern</label>
        <div class="col-sm-10">
            <select id="ledPattern" class="form-control" #selectedLedPattern>
                <option *ngFor="let pattern of ledPatterns" [value]="pattern.id">{{pattern.name}}</option>
            </select>
        </div>
    </div>
    <div class="form-group">
        <div class="col-sm-offset-2 col-sm-10">
            <button type="submit" class="btn btn-primary" (click)="playLights(selectedLedPattern.value)">Play Lights</button>
            <button type="submit" class="btn btn-primary" (click)="stopLights()">Stop Lights</button>
        </div>
    </div>
    <div class="form-group">
        <label for="audioPattern" class="col-sm-2 control-label">Audio Pattern</label>
        <div class="col-sm-10">
            <select id="audioPattern" class="form-control">
                <option *ngFor="let pattern of audioPatterns" [value]="pattern.id">{{pattern.name}}</option>
            </select>
        </div>
    </div>
    <div class="form-group">
        <div class="col-sm-offset-2 col-sm-10">
            <button type="submit" class="btn btn-primary" (click)="playAudio(selectedLedPattern.value)">Play Audio</button>
            <button type="submit" class="btn btn-primary" (click)="stopAudio()">Stop Audio</button>
        </div>
    </div>
</form>
`
})
export class ShowOff {
    constructor(private getSirenInfoCommand: GetSirenInfoCommand) {
        getSirenInfoCommand
            .getSirenInfo()
            .then(result => {
                this.ledPatterns = result.ledPatterns;
                this.audioPatterns = result.audioPatterns;
            });
    }

    public ledPatterns:string[];
    public audioPatterns: string[];

    public playLights(id: number) {
        this.getSirenInfoCommand.playLedPattern(id);
    }

    public stopLights() {
        this.getSirenInfoCommand.playLedPattern(null);
    }

    public playAudio(id: number) {
        this.getSirenInfoCommand.playAudioPattern(id);
    }

    public stopAudio() {
        this.getSirenInfoCommand.playAudioPattern(null);
    }
}