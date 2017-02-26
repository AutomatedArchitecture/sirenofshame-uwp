import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CiEntryPointSetting } from './models/ciEntryPointSetting';
import { GetBuildDefinitionsCommand } from './commands/get-builddefinitions.command';
import { GetCiEntryPointSettingCommand } from './commands/get-cientrypointsetting.command';
import { AddCiEntryPointSettingCommand } from './commands/add-cientrypointsetting.command';
import { MyBuildDefinition } from './models/myBuildDefinition';
import { ServerService } from './server.service';
import { CiEntryPoint } from './models/ciEntryPoint.ts'
import { GetCiEntryPointsCommand } from './commands/get-cientrypoints.command'

@Component({
    templateUrl: './components/server.html'
})
export class Server {
    constructor(
        private getBuildDefinitionsCommand: GetBuildDefinitionsCommand,
        private getCiEntryPointSettingCommand: GetCiEntryPointSettingCommand,
        private addCiEntryPointSettingCommand: AddCiEntryPointSettingCommand,
        private route: ActivatedRoute,
        private router: Router,
        private serverService: ServerService,
        private getCiEntryPointsCommand: GetCiEntryPointsCommand
    ) {

    }

    private sub: any;
    public serverTypes: CiEntryPoint[];

    ngOnInit() {
        this.addingProjects = false;

        this.sub = this.route.params.subscribe(params => {
            let id = +params['id'];
            if (id) {
                this.getCiEntryPointSettingCommand.invoke(id)
                    .then(ciEntryPointSetting => {
                        this.ciEntryPointSetting.id = ciEntryPointSetting.id;
                        this.ciEntryPointSetting.name = ciEntryPointSetting.name;
                        this.ciEntryPointSetting.url = ciEntryPointSetting.url;
                    });
            }
        });

        this.getCiEntryPointsCommand.execute()
            .then(ciEntryPoints => {
                this.serverTypes = ciEntryPoints;
            });
    }

    ngOnDestroy() {
        this.sub.unsubscribe();
    }

    public ciEntryPointSetting = new CiEntryPointSetting();

    public loadingProjects: boolean = false;
    public addingProjects: boolean = false;

    public errorMessage: string = null;

    public getProjects() {
        this.loadingProjects = true;
        this.getBuildDefinitionsCommand.getBuildDefinitions(this.ciEntryPointSetting)
            .then(projects => {
                this.ciEntryPointSetting.projects = projects;
                this.addingProjects = true;
            }, ex => {
                this.errorMessage = ex;
            })
            .then(() => {
                this.loadingProjects = false;
            });
    }

    public onAddBuilds() {
        this.addingProjects = false;
    }

    public onSave() {
        this.ciEntryPointSetting.buildDefinitionSettings = this.ciEntryPointSetting.projects
            .filter(project => project.selected);

        this.addCiEntryPointSettingCommand.invoke(this.ciEntryPointSetting)
            .then((ciEntryPointSettingId: number) => {
                let adding = this.ciEntryPointSetting.id === 0;
                if (adding) {
                    this.ciEntryPointSetting.id = ciEntryPointSettingId;
                    this.serverService.serverAdded.emit((this.ciEntryPointSetting));
                }
                this.router.navigate(['home']);
            });
    }
}