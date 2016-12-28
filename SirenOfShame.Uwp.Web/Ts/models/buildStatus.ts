export interface BuildStatus {
    buildDefinitionId: string;
    buildId: string;
    startedTime: Date;
    buildStatusEnum: number;
    buildStatusMessage: string;
    comment: string;
    finishedTime: Date;
    localStartTime: Date;
    name: string;
    requestedBy: string;
    url: string;
}

export interface BuildStatusEnum {
    id: number;
    title: string;
}