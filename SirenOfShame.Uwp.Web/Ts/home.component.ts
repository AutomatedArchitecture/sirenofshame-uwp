import {Component, OnInit} from '@angular/core';
import { ServerService } from './server.service';

@Component({
    template: `
    <h1>Siren of Shame</h1>
    <h2>Messages</h2>
    <div>{{messages}}</div>
    <h2>Errors</h2>
    <div>{{errors}}</div>
    <h2>Send</h2>
    <input type="text" [(ngModel)]="message" />
    <button type="submit" (click)="onButtonClick()">Send</button>
`
})
export class Home implements OnInit {
    public message: string;
    public messages: string;
    public errors: string;

    constructor(private serverService: ServerService) {
        serverService.onMessage = (message) => {
            this.messages += message;
        }
    }

    public onButtonClick() {
        this.serverService.send(this.message);
    }

    public ngOnInit() {
        this.messages = '';

    }
}
