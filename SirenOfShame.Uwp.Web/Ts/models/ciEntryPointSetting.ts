import { MyBuildDefinition } from './myBuildDefinition';

export class CiEntryPointSetting {
    public id: number;
    public name: string;
    public url: string;
    public username: string;
    public password: string;
    public projects: MyBuildDefinition[];
    public buildDefinitionSettings: MyBuildDefinition[];
}