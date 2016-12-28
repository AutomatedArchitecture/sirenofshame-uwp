import {Component} from '@angular/core';

@Component({
    template: `
<h2>Project #1</h2>

<form class="form-horizontal">
  <div class="form-group">
    <label for="inputStatus" class="col-sm-2 control-label">Status</label>
    <div class="col-sm-10">
      <select class="form-control" id="Status" placeholder="Status">
        <option>Unknown</option>
        <option>Working</option>
        <option>Broken</option>
        <option>In Progress</option>
      </select>
    </div>
  </div>
  <div class="form-group">
    <label for="inputComment" class="col-sm-2 control-label">Comment</label>
    <div class="col-sm-10">
      <input type="text" class="form-control" id="inputComment" placeholder="Comment">
    </div>
  </div>
  <div class="form-group">
    <label for="inputRequestedBy" class="col-sm-2 control-label">Requested By</label>
    <div class="col-sm-10">
      <input type="text" class="form-control" id="inputRequestedBy" placeholder="Requested By">
    </div>
  </div>
  <div class="form-group">
    <div class="col-sm-offset-2 col-sm-10">
      <button type="submit" class="btn btn-primary" (click)="send()">Send</button>
    </div>
  </div>
</form>

<h2>Project #2</h2>
<h2>Project #3</h2>
`
})
export class MockServer {
    constructor() {
    }

    public send() {
        alert('sending');
    }
}