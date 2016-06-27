import {Component } from '@angular/core';
import {CiServer} from './models/ciServer';
import { ServerService } from './server.service';

@Component({
    templateUrl: './components/server.html'
})
export class Server {
    constructor(private serverService: ServerService) {
        
    }

    public ciServer = new CiServer();

    public onSubmit() {
        this.serverService.getProjects(this.ciServer).then(projects => {
            alert('got ' + projects.length + 'projects');
        });
    }
}