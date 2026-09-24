import { Component } from '@angular/core';
import { AppHeader } from '../header/header.component';
import { RouterOutlet } from '@angular/router';

@Component({
  standalone: true,
  imports: [AppHeader, RouterOutlet],
  selector: 'app-main-layout',
  styleUrl: './main-layout.component.scss',
  templateUrl: './main-layout.component.html',
})
export class AppMainLayout {}
