import {Component } from '@angular/core';

@Component({
    template: `
    <h1>Add CI Server</h1>
    <form class="form-horizontal">
        <div class="form-group">
            <label for="serverType" class="col-sm-2 control-label">Server Type</label>
            <div class="col-sm-10">
                <select class="form-control" id="serverType">
                    <option></option>
                    <option>Jenkins</option>
                </select>
            </div>
        </div>
        <div class="form-group">
            <label for="url" class="col-sm-2 control-label">URL</label>
            <div class="col-sm-10">
                <input type="text" class="form-control" id="url" />
            </div>
        </div>
        <div class="form-group">
            <label for="username" class="col-sm-2 control-label">User Name</label>
            <div class="col-sm-10">
                <input type="text" class="form-control" id="username" />
            </div>
        </div>
        <div class="form-group">
            <label for="password" class="col-sm-2 control-label">Password</label>
            <div class="col-sm-10">
                <input type="password" class="form-control" id="password" />
            </div>
        </div>
        <div class="form-group">
            <div class="col-sm-offset-2 col-sm-10">
                <div class="checkbox">
                    <label>
                        <input type="checkbox"> Treat unstable as success
                    </label>
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="col-sm-offset-2 col-sm-10">
                <button type="submit" class="btn btn-default">Connect</button>
            </div>
        </div>
    </form>
`
})
export class Server {
    
}