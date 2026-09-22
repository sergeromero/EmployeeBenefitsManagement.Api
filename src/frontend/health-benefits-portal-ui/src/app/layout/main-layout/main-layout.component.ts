import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from '../header/header.component';

@Component({
  standalone: true,
  imports: [RouterOutlet, Header],
  selector: 'app-main-layout',
  styleUrl: './main-layout.component.scss',
  templateUrl: './main-layout.component.html',
})
export class MainLayout {}
