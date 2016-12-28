import {Component, NgModule} from '@angular/core';
import {BuildStatus, BuildStatusEnum} from "../models/buildStatus";
import {MockBuild} from './mock-build.component'

@Component({
    template: `
<h2>Project #1</h2>
<mock-build [buildStatuses]="buildStatuses" [buildStatus]="buildStatus1"></mock-build>
<h2>Project #2</h2>
<mock-build [buildStatuses]="buildStatuses" [buildStatus]="buildStatus2"></mock-build>
<h2>Project #3</h2>
<mock-build [buildStatuses]="buildStatuses" [buildStatus]="buildStatus3"></mock-build>
`
})
export class MockServer {
    constructor() {
    }

    public buildStatuses: BuildStatusEnum[] = [
        { id: 0, title: 'Unknown' },
        { id: 1, title: 'Working' },
        { id: 2, title: 'Broken' },
        { id: 3, title: 'In Progress' }
    ];

    public buildStatus1: BuildStatus = this.makeBuildStatus();
    public buildStatus2: BuildStatus = this.makeBuildStatus();
    public buildStatus3: BuildStatus = this.makeBuildStatus();

    private makeBuildStatus(): BuildStatus {
        return {
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
        }
    }
}