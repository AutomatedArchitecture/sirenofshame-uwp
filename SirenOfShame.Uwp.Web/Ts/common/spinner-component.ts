import {Component, Input} from "@angular/core";

@Component({
    selector: "app-spinner",
    template: `<div class="spinner" [style.opacity]="spinnerOpacity">
      <div class="sk-double-bounce">
        <div class="sk-child sk-double-bounce1"></div>
        <div class="sk-child sk-double-bounce2"></div>
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
        var newOpacity = value ? 1 : 0;
        if (this.initialLoad) {
            this.initialLoad = false;
            setTimeout(() => { this.spinnerOpacity = newOpacity; }, 1);
        } else {
            this.spinnerOpacity = newOpacity;
        }
    }

    public spinnerOpacity: number;
    private initialLoad: boolean = true;
}