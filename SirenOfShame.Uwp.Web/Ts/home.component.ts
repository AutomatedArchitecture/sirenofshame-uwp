import {Component, OnInit} from '@angular/core';

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
    private ws;
    public message: string;
    public messages: string;
    public errors: string;

    public onButtonClick() {
        this.ws.send(this.message);
    }

    public ngOnInit() {
        this.messages = '';
        if ("WebSocket" in window) {
            var wsUrl = "ws://" + location.hostname + (location.port ? ':' + location.port : '') + "/sockets/";
            var connection = new WebSocket(wsUrl, ['echo']);
            connection.onopen = () => {
                this.ws = connection;
                this.ws.onmessage = (e) => {
                    this.messages += e.data;
                };
                this.errors = "Connection established";
                this.ws.send("Hello world!");
            };
            connection.onerror = (error) => {
                this.errors = "Could not connect to websocket. " + error;
            };
        }
        else {
            this.errors = "This browser does not support websockets!";
        }
    }
}
