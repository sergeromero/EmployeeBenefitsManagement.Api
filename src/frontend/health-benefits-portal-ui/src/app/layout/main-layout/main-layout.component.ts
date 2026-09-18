import { Component } from '@angular/core';
import { AppButton } from '../../shared/ui/button';

@Component({
  standalone: true,
  imports: [AppButton],
  selector: 'app-main-layout',
  styleUrl: './main-layout.component.scss',
  templateUrl: './main-layout.component.html',
})
export class MainLayout {}
