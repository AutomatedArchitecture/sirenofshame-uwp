import {Component, Input } from '@angular/core';
import {BuildStatus, BuildStatusEnum} from "../models/buildStatus";
import {UpdateMockBuildCommand} from './update-mock-build.command';

@Component({
    selector: 'mock-build',
    template: `
<h2>{{buildStatus.name}}</h2>
<form class="form-horizontal" novalidate #serverForm="ngForm" (ngSubmit)="send()">
  <div class="form-group">
    <label for="inputStatus" class="col-sm-2 control-label">Status</label>
    <div class="col-sm-10">
      <select [(ngModel)]="buildStatus.buildStatusEnum" class="form-control" id="Status" name="Status" placeholder="Status">
        <option *ngFor="let buildStatus of buildStatuses" [value]="buildStatus.id">{{buildStatus.title}}</option>
      </select>
    </div>
  </div>
  <div class="form-group">
    <label for="requestedBy" class="col-sm-2 control-label">Requested By</label>
    <div class="col-sm-10">
      <input type="text" class="form-control" [(ngModel)]="buildStatus.requestedBy" id="requestedBy" name="requestedBy" placeholder="Requested By">
    </div>
  </div>
  <div class="form-group">
    <label for="comment" class="col-sm-2 control-label">Comment</label>
    <div class="col-sm-10">
      <textarea rows="4" [(ngModel)]="buildStatus.comment" class="form-control" id="comment" name="comment" placeholder="Comment"></textarea>
    </div>
  </div>
  <div class="form-group">
    <div class="col-sm-offset-2 col-sm-10">
      <button type="submit" class="btn btn-primary">Send</button>
    </div>
  </div>
</form>
`
})
export class MockBuild {
    constructor(private updateMockBuildCommand: UpdateMockBuildCommand) {
    }

    @Input()
    public buildStatuses: BuildStatusEnum[];

    @Input()
    public buildStatus: BuildStatus;

    public send() {
        this.updateMockBuildCommand.execute(this.buildStatus);
    }
}