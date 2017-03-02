import { Component, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { CiEntryPointSetting } from "../models/ciEntryPointSetting";
import { GetBuildDefinitionsCommand } from "../commands/get-builddefinitions.command";
import { GetCiEntryPointSettingCommand } from "../commands/get-cientrypointsetting.command";
import { AddCiEntryPointSettingCommand } from "../commands/add-cientrypointsetting.command";
import { ServerService } from "../services/server.service";
import { CiEntryPoint } from "../models/ciEntryPoint.ts";
import { GetCiEntryPointsCommand } from "../commands/get-cientrypoints.command";
import { DeleteServerCommand } from "../commands/delete-server.command";
import { ModalComponent } from "../common/modal-component";
import { MyBuildDefinition } from "../models/myBuildDefinition";

@Component({
    templateUrl: "./components/server.html"
})
export class Server {
    constructor(
        private getBuildDefinitionsCommand: GetBuildDefinitionsCommand,
        private getCiEntryPointSettingCommand: GetCiEntryPointSettingCommand,
        private addCiEntryPointSettingCommand: AddCiEntryPointSettingCommand,
        private deleteServerCommand: DeleteServerCommand,
        private route: ActivatedRoute,
        private router: Router,
        private serverService: ServerService,
        private getCiEntryPointsCommand: GetCiEntryPointsCommand
    ) {

    }

    private sub: any;
    public serverTypes: CiEntryPoint[];
    @ViewChild(ModalComponent)
    public modal: ModalComponent;

    ngOnInit() {
        this.addingProjects = false;

        this.sub = this.route.params.subscribe(params => {
            let id = +params["id"];

            if (id) {
                this.editingExisting = true;
                this.getCiEntryPointSettingCommand.invoke(id)
                    .then((ciEntryPointSetting: CiEntryPointSetting) => {
                        this.ciEntryPointSetting.id = ciEntryPointSetting.id;
                        this.ciEntryPointSetting.name = ciEntryPointSetting.name;
                        this.ciEntryPointSetting.url = ciEntryPointSetting.url;
                        this.ciEntryPointSetting.buildDefinitionSettings = ciEntryPointSetting.buildDefinitionSettings;
                    });
            }
        });

        this.getCiEntryPointsCommand.execute()
            .then((ciEntryPoints: CiEntryPoint[]) => {
                this.serverTypes = ciEntryPoints;
            });
    }

    ngOnDestroy() {
        this.sub.unsubscribe();
    }

    public ciEntryPointSetting = new CiEntryPointSetting();

    public loadingProjects: boolean = false;
    public addingProjects: boolean = false;
    public editingExisting: boolean = false;

    public errorMessage: string = null;
    public allProjects: MyBuildDefinition[];

    public getProjects() {
        this.loadingProjects = true;
        this.getBuildDefinitionsCommand.getBuildDefinitions(this.ciEntryPointSetting)
            .then((projects: MyBuildDefinition[]) => {
                this.allProjects = projects.filter(p => !this.ciEntryPointSetting.buildDefinitionSettings.some(b => b.id === p.id));
                this.addingProjects = true;
            }, ex => {
                this.errorMessage = ex;
            })
            .then(() => {
                this.loadingProjects = false;
            });
    }

    public delete() {
        this.deleteServerCommand.execute(this.ciEntryPointSetting.id).then(() => {
            this.serverService.refreshCiEntryPoints.emit();
            this.router.navigate(["home"]);
            }
        );
    }

    public deleteProject(project: MyBuildDefinition) {
        var filteredList = this.ciEntryPointSetting.buildDefinitionSettings.filter(i => i.name !== project.name);
        this.ciEntryPointSetting.buildDefinitionSettings = filteredList;
    }

    public onBuildsAdded(builds: MyBuildDefinition[]) {
        this.addingProjects = false;
        let combinedList = this.ciEntryPointSetting.buildDefinitionSettings.concat(builds);
        this.ciEntryPointSetting.buildDefinitionSettings = combinedList;
    }

    public onSave() {
        this.addCiEntryPointSettingCommand.invoke(this.ciEntryPointSetting)
            .then((ciEntryPointSettingId: number) => {
                let adding = this.ciEntryPointSetting.id === 0;
                if (adding) {
                    this.ciEntryPointSetting.id = ciEntryPointSettingId;
                }
                this.serverService.refreshCiEntryPoints.emit();
                this.router.navigate(["home"]);
            });
    }
}