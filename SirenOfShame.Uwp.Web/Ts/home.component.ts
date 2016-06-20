import {Component, OnInit } from '@angular/core';
import { ServerService } from './server.service';

@Component({
    template: `
    <h1>Siren of Shame</h1>
    <h2>Messages</h2>
    <div>{{messages}}</div>
    <h2 *ngIf="connectionStatus">Status</h2>
    <div *ngIf="connectionStatus">{{connectionStatus}}</div>
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
        serverService.connected.subscribe(() => this.connectionStatus = null);
        serverService.connectionError.subscribe(error => this.connectionStatus = error);
    }

    public connectionStatus: string = 'Connecting to server ...';

    public onButtonClick() {
        this.serverService
            .echo(this.message)
            .then(message => this.messages += message, err => alert(err));
    }

    public ngOnInit() {
        this.messages = '';

    }
}
