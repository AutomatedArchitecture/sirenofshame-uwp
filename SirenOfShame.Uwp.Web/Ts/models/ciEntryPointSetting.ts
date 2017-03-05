import { MyBuildDefinition } from './myBuildDefinition';

export class CiEntryPointSetting {
    public id: number;
    public name: string;
    public url: string;
    public userName: string;
    public password: string;
    public buildDefinitionSettings: MyBuildDefinition[];
}