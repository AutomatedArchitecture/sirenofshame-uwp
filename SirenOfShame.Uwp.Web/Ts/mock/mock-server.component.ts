import {Component, NgModule} from '@angular/core';
import {BuildStatus, BuildStatusEnum} from "../models/buildStatus";
import {MockBuild} from './mock-build.component'

@Component({
    template: `
<mock-build [buildStatuses]="buildStatuses" [buildStatus]="buildStatus1"></mock-build>
<mock-build [buildStatuses]="buildStatuses" [buildStatus]="buildStatus2"></mock-build>
<mock-build [buildStatuses]="buildStatuses" [buildStatus]="buildStatus3"></mock-build>
`
})
export class MockServer {
    public buildStatuses: BuildStatusEnum[] = [
        { id: 0, title: 'Unknown' },
        { id: 1, title: 'Working' },
        { id: 2, title: 'Broken' },
        { id: 3, title: 'In Progress' }
    ];

    public buildStatus1: BuildStatus = this.makeBuildStatus(1);
    public buildStatus2: BuildStatus = this.makeBuildStatus(2);
    public buildStatus3: BuildStatus = this.makeBuildStatus(3);

    private makeBuildStatus(id: number): BuildStatus {
        return {
            buildDefinitionId: 'Mock' + id,
            buildId: new Date().getMilliseconds().toString(),
            startedTime: new Date(),
            buildStatusEnum: 1,
            buildStatusMessage: 'What is this?',
            comment: 'From the UI!!!',
            finishedTime: new Date(),
            localStartTime: new Date(),
            name: 'Project #' + id,
            requestedBy: 'BobSmith',
            url: 'http://www.google.com'
        }
    }
}