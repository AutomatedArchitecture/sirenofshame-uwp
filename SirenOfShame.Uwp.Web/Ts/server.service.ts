import { Injectable, Output, EventEmitter } from '@angular/core';
import { MyBuildDefinition } from './models/myBuildDefinition';
import { CiServer } from './models/ciServer';
import { BaseCommand } from './commands/base.command';

interface ISirenInfo {
    ledPatterns;
    audioPatterns;
}

@Injectable()
export class ServerService {
    private ws;
    private commands: BaseCommand[] = [];

    constructor() {
        this.isConnected = false;
        if ("WebSocket" in window) {
            this.connectToServer();
        }
        else {
            this.connectionError.emit("This browser does not support websockets");
        }
    }

    registerCommand(command: BaseCommand) {
        this.commands.push(command);
    }

    private connectToServer() {
        var wsUrl = this.getUrl();
        var connection = new WebSocket(wsUrl, ['echo']);
        connection.onopen = () => {
            this.isConnected = true;
            this.ws = connection;
            this.ws.onmessage = (e) => {
                let data = JSON.parse(e.data);

                this.commands.forEach(command => {
                    if (command.type === data.type) {
                        command.response(data);
                    }
                });

                if (data.type === 'getSirenInfo') {
                    this.onGetSirenInfo(data);
                }
                if (data.type === 'getProjects') {
                    this.onGetProjects(data);
                }
                if (data.type === 'deviceConnectionChanged') {
                    if (data.responseCode === 200) {
                        this.deviceConnectionChanged.emit(data.result);
                    } else {
                        console.error(data.result);
                    }
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

    @Output() public deviceConnectionChanged: EventEmitter<boolean> = new EventEmitter<any>(true);

    public isConnected: boolean;

    private onGetSirenInfo;
    private onGetProjects;

    private getUrl() {
        //let port = (location.port ? ':' + location.port : '');
        let port = ':8001';
        let hostname = location.hostname;

        return "ws://" + hostname + port + "/sockets/";
    }

    public send(sendRequest, err?: any) {
        if (this.ws && this.isConnected) {
            this.ws.send(JSON.stringify(sendRequest));
        } else {
            var subscription = this.connected.subscribe(() => {
                this.ws.send(JSON.stringify(sendRequest));
                subscription.unsubscribe();
            });
        }
    }

    public getSirenInfo(): Promise<ISirenInfo> {
        return new Promise<ISirenInfo>((resolve, err) => {
            this.onGetSirenInfo = (sirenInfo) => resolve(sirenInfo.result);
            var sendRequest = {
                type: 'getSirenInfo'
            }
            this.send(sendRequest, err);
        }
        );
    }

    public getProjects(ciServer: CiServer): Promise<MyBuildDefinition[]> {
        return new Promise<MyBuildDefinition[]>((resolve, err) => {
            this.onGetProjects = (result) => {
                if (result.responseCode === 200) {
                    resolve(result.result);
                } else {
                    err(result.result);
                }
            };
            var sendRequest = {
                type: 'getProjects',
                ciServer: ciServer
            }
            this.send(sendRequest, err);
        });
    }

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