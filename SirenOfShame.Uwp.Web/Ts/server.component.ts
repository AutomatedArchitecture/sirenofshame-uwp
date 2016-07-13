import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CiEntryPointSetting } from './models/ciEntryPointSetting';
import { GetBuildDefinitionsCommand } from './commands/get-builddefinitions.command';
import { GetCiEntryPointSettingCommand } from './commands/get-cientrypointsetting.command';
import { AddCiEntryPointSettingCommand } from './commands/add-cientrypointsetting.command';
import { MyBuildDefinition } from './models/myBuildDefinition';

@Component({
    templateUrl: './components/server.html'
})
export class Server {
    constructor(
        private getBuildDefinitionsCommand: GetBuildDefinitionsCommand,
        private getCiEntryPointSettingCommand: GetCiEntryPointSettingCommand,
        private addCiEntryPointSettingCommand: AddCiEntryPointSettingCommand,
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
                        this.ciEntryPointSetting.url = ciEntryPointSetting.url;
                    });
            }
        });
    }

    ngOnDestroy() {
        this.sub.unsubscribe();
    }

    public ciEntryPointSetting = new CiEntryPointSetting();

    public loadingProjects: boolean = false;

    public errorMessage: string = null;

    public getProjects() {
        this.loadingProjects = true;
        this.getBuildDefinitionsCommand.getBuildDefinitions(this.ciEntryPointSetting)
            .then(projects => {
                this.ciEntryPointSetting.projects = projects;
            }, ex => {
                this.errorMessage = ex;
            })
            .then(() => {
                this.loadingProjects = false;
            });
    }

    public onSave() {
        this.addCiEntryPointSettingCommand.invoke(this.ciEntryPointSetting)
            .then(() => this.router.navigate(['home']));
    }
}