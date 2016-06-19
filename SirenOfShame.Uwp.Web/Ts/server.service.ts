import { Injectable } from '@angular/core';

interface ISirenInfo {
    ledPatterns;
    audioPatterns;
}

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
                    let data = JSON.parse(e.data);
                    if (data.type === 'echoResult') {
                        this.onMessage(data.result);
                        return;
                    } 
                    if (data.type === 'getSirenInfoResult') {
                        this.onGetSirenInfo(data.result);
                    }
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
                var sendRequest = {
                    type: 'echo',
                    message: message
                };
                this.ws.send(JSON.stringify(sendRequest));
            }
        );
    }

    public getSirenInfo(): Promise<ISirenInfo> {
        return new Promise<ISirenInfo>((resolve) => {
            this.onGetSirenInfo = (sirenInfo) => resolve(sirenInfo);
            var sendRequest = {
                type: 'getSirenInfo'
            }
            this.ws.send(JSON.stringify(sendRequest));
        }
        );
    }

    private onMessage;
    private onGetSirenInfo;
    public connectionStatus: string;
}