import { Component } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { AppHeader } from '../header/header.component';

@Component({
    selector: "app-auth-layout",
    imports: [RouterOutlet, AppHeader],
    template: `
    <app-header></app-header>
        <main>
            <router-outlet></router-outlet>
        </main>
    `
})
export class AppAuthLayout {}