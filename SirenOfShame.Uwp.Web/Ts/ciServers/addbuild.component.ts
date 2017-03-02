import { Component, Input, Output, EventEmitter } from "@angular/core";
import { MyBuildDefinition } from "../models/myBuildDefinition";

@Component({
    selector: "app-addbuild",
    template: `<form (ngSubmit)="onAddBuilds()" #serverForm="ngForm" novalidate>
    <p>Select projects to watch</p>
    <div class="checkbox" *ngFor="let project of projects">
        <label>
            <input type="checkbox" [(ngModel)]="project.selected" name="{{project.id}}"> {{project.name}}
        </label>
    </div>
    <button type="submit" class="btn btn-default">Add</button>
</form>
`
})
export class AddBuild {
    @Input()
    public projects: MyBuildDefinition[];

    @Output()
    buildsAdded: EventEmitter<MyBuildDefinition[]> = new EventEmitter<MyBuildDefinition[]>();

    public onAddBuilds() {
        let selectedProjects = this.projects.filter(i => i.selected);
        this.buildsAdded.emit(selectedProjects);
    }
}