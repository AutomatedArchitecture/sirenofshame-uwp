import {Component, Input} from "@angular/core";

@Component({
    selector: "app-spinner",
    template: `<div *ngIf="spinnerBlocksInput" class="spinner-wrapper" [style.opacity]="spinnerOpacity">
        <div class="spinner">
          <div class="sk-double-bounce">
            <div class="sk-child sk-double-bounce1"></div>
            <div class="sk-child sk-double-bounce2"></div>
          </div>
        </div>
    </div>
`
})
export class Spinner {
    constructor() {
        this.spinnerOpacity = 0;
    }

    @Input()
    public set isBusy(value: boolean) {
        if (this.initialLoad) {
            this.initialLoad = false;
            // the very first load might happen in a constructor or ngInit so wait until the page has loaded
            setTimeout(() => { this.setIsBusy(value); }, 1);
        } else {
            this.setIsBusy(value);
        }
    }

    private setIsBusy(newIsBusy: boolean) {
        if (newIsBusy) {
            this.spinnerBlocksInput = true;
        } else {
            // if we're turning the spinner back off wait until the animation completes then hide it
            setTimeout(() => { this.spinnerBlocksInput = false; }, 400);
        }
        var newOpacity = newIsBusy ? 1 : 0;
        this.spinnerOpacity = newOpacity;
    }

    public spinnerOpacity: number;
    public spinnerBlocksInput: boolean = false;
    private initialLoad: boolean = true;
}