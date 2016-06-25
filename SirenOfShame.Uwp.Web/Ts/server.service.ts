import { Injectable, Output, EventEmitter } from '@angular/core';

interface ISirenInfo {
    ledPatterns;
    audioPatterns;
}

@Injectable()
export class ServerService {
    private ws;

    constructor() {
        this.isConnected = false;
        if ("WebSocket" in window) {
            this.connectToServer();
        }
        else {
            this.connectionError.emit("This browser does not support websockets");
        }
    }

    private connectToServer() {
        var wsUrl = this.getUrl();
        var connection = new WebSocket(wsUrl, ['echo']);
        connection.onopen = () => {
            this.isConnected = true;
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

            this.connected.emit(null);
        };
        connection.onerror = () => {
            this.isConnected = false;
            this.connectionError.emit("Error connecting via websockets.");
        };
        connection.onclose = () => {
            this.isConnected = false;
            this.connectionError.emit("Lost connection to server, attempting to reconnect.");
            this.connectToServer();
        }
    }

    @Output() public connected: EventEmitter<any> = new EventEmitter<any>(true);
    @Output() public connectionError: EventEmitter<string> = new EventEmitter<string>(true);
    public isConnected: boolean;

    private getUrl() {
        //let port = (location.port ? ':' + location.port : '');
        let port = ':8001';
        let hostname = location.hostname;

        return "ws://" + hostname + port + "/sockets/";
    }

    private send(sendRequest, err?: any) {
        if (this.ws && this.isConnected) {
            this.ws.send(JSON.stringify(sendRequest));
        } else {
            var subscription = this.connected.subscribe(() => {
                this.ws.send(JSON.stringify(sendRequest));
                subscription.unsubscribe();
            });
        }
    }

    public echo(message: string): Promise<string> {
        return new Promise<string>((resolve, err) => {
                this.onMessage = (message) => resolve(message);
                var sendRequest = {
                    type: 'echo',
                    message: message
                };
                this.send(sendRequest, err);
            }
        );
    }

    public getSirenInfo(): Promise<ISirenInfo> {
        return new Promise<ISirenInfo>((resolve, err) => {
            this.onGetSirenInfo = (sirenInfo) => resolve(sirenInfo);
            var sendRequest = {
                type: 'getSirenInfo'
            }
            this.send(sendRequest, err);
        }
        );
    }

    private onMessage;
    private onGetSirenInfo;

    public playLedPattern(id: number) {
        var sendRequest = {
            type: 'playLedPattern',
            id: id
        }
        this.send(sendRequest);
    }

    public playAudioPattern(id: number) {
        var sendRequest = {
            type: 'playAudioPattern',
            id: id
        }
        this.send(sendRequest);
    }
}