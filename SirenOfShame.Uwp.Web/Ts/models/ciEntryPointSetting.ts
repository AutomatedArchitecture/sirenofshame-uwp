import { MyBuildDefinition } from './myBuildDefinition';

export class CiEntryPointSetting {
    public id: number;
    public name: string;
    public url: string;
    public serverType: string;
    public projects: MyBuildDefinition[];
}