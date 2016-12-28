import {Component} from '@angular/core';
import {UpdateMockBuildCommand} from './commands/update-mock-build.command';
import {BuildStatus, BuildStatusEnum} from "./models/buildStatus";

@Component({
    template: `
<h2>Project #1</h2>

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

<h2>Project #2</h2>
<h2>Project #4</h2>
`
})
export class MockServer {
    constructor(private updateMockBuildCommand: UpdateMockBuildCommand) {
    }

    public buildStatuses: BuildStatusEnum[] = [
        { id: 0, title: 'Unknown' },
        { id: 1, title: 'Working' },
        { id: 2, title: 'Broken' },
        { id: 3, title: 'In Progress' }
    ];

    public buildStatus: BuildStatus = {
        buildDefinitionId: 'Mock1',
        buildId: new Date().getMilliseconds().toString(),
        startedTime: new Date(),
        buildStatusEnum: 2,
        buildStatusMessage: 'What is this?',
        comment: 'From the UI!!!',
        finishedTime: new Date(),
        localStartTime: new Date(),
        name: 'Mock 1',
        requestedBy: 'BobSmith',
        url: 'http://www.google.com'
    };

    public send() {
        this.updateMockBuildCommand.execute(this.buildStatus);
    }
}