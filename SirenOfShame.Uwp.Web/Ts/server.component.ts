import {Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {CiServer} from './models/ciServer';
import { GetBuildDefinitionsCommand } from './commands/get-builddefinitions.command';
import { GetCiEntryPointSettingCommand } from './commands/get-cientrypointsetting.command';
import { MyBuildDefinition } from './models/myBuildDefinition';

@Component({
    templateUrl: './components/server.html'
})
export class Server {
    constructor(
        private getBuildDefinitionsCommand: GetBuildDefinitionsCommand,
        private getCiEntryPointSettingCommand: GetCiEntryPointSettingCommand,
        private route: ActivatedRoute,
        private router: Router
    ) {
        
    }

    private sub: any;

    ngOnInit() {
        this.sub = this.route.params.subscribe(params => {
            let id = +params['id'];
            if (id) {
                this.getCiEntryPointSettingCommand.invoke(id)
                    .then(ciEntryPointSetting => {
                        this.ciServer.url = ciEntryPointSetting.url;
                    });
            }
        });
    }

    ngOnDestroy() {
        this.sub.unsubscribe();
    }

    public ciServer = new CiServer();

    public projects: MyBuildDefinition[];

    public loadingProjects: boolean = false;

    public errorMessage: string = null;

    public getProjects() {
        this.loadingProjects = true;
        this.getBuildDefinitionsCommand.getBuildDefinitions(this.ciServer)
            .then(projects => {
                this.projects = projects;
            }, ex => {
                this.errorMessage = ex;
            })
            .then(() => {
                this.loadingProjects = false;
            });
    }

    public onSave() {
        this.projects.forEach(project => {
            if (project.selected) {
                alert('you selected ' + project.name);
            }
        });
    }
}