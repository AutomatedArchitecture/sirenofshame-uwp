import {Component} from '@angular/core';
import {Routes, Router, Route, ROUTER_DIRECTIVES} from '@angular/router';
import {ShowOff} from './show-off.component'

@Component({
    selector: 'my-app',
    templateUrl: 'components/app.html',
    directives: [ROUTER_DIRECTIVES]
})
@Routes([
    new Route({ path: '/showoff', component: ShowOff })
])
export class AppComponent {
    constructor(private router: Router) {
    }

    public onClick() {
        this.router.navigate(['/showoff']);
    }
}