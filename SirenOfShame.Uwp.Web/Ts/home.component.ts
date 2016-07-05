import {Component, OnInit } from '@angular/core';
import { EchoCommand } from './commands/echo.command'

@Component({
    template: `
    <h1>Siren of Shame</h1>
    <h2>Messages</h2>
    <div>{{messages}}</div>
    <h2>Send</h2>
    <input type="text" [(ngModel)]="message" />
    <button type="submit" (click)="onButtonClick()">Send</button>
`
})
export class Home implements OnInit {
    public message: string;
    public messages: string;
    public errors: string;

    constructor(private echoCommand: EchoCommand) {
    }

    public connectionStatus: string;

    public onButtonClick() {
        this.echoCommand
            .echo(this.message)
            .then(message => this.messages += message, err => alert(err));
    }

    public ngOnInit() {
        this.messages = '';

    }
}
