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
        this.setCurrentUrl();
    }

    public currentUrl: string;

    private setCurrentUrl() {
        let url = this.router.serializeUrl(this.router.urlTree);
        if (url)
            this.currentUrl = url;
        else
            this.currentUrl = '/';
    }

    public gotoHome() {
        this.router.navigate(['/']);
        this.setCurrentUrl();
    }

    public gotoShowOff() {
        this.router.navigate(['/showoff']);
        this.setCurrentUrl();
    }
}