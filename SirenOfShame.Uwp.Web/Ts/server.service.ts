import { Injectable } from '@angular/core';

@Injectable()
export class ServerService {
    private ws;

    constructor() {
        if ("WebSocket" in window) {
            var wsUrl = this.getUrl();

            var connection = new WebSocket(wsUrl, ['echo']);
            connection.onopen = () => {
                this.ws = connection;
                this.ws.onmessage = (e) => {
                    this.onMessage(e.data);
                };

                this.connectionStatus = "Connection established";
            };
            connection.onerror = (error) => {
                this.connectionStatus = "Could not connect to websocket. " + error;
            };
        }
        else {
            this.connectionStatus = "This browser does not support websockets";
        }
    }

    private getUrl() {
        //return "ws://" + location.hostname + (location.port ? ':' + location.port : '') + "/sockets/"
        return "ws://LeesRasPi3:8001/sockets/";
    }

    public send(message: string): Promise<string> {
        return new Promise<string>((resolve) => {
                this.onMessage = (message) => resolve(message);
                this.ws.send(message);
            }
        );
    }

    private onMessage;
    public connectionStatus: string;
}