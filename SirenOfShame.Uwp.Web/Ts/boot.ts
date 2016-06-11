///<reference path="./../typings/browser/ambient/es6-shim/index.d.ts"/>
import {bootstrap}    from '@angular/platform-browser-dynamic';
import {AppComponent} from './app';
import {ROUTER_PROVIDERS} from '@angular/router';

bootstrap(AppComponent, [
    ROUTER_PROVIDERS
]).catch(err => console.error(err));
