import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
  //  template: `
  //   <div style="background: blue; color: white; padding: 40px; margin: 20px;">
  //     <h1>DASHBOARD CONTENT HERE!</h1>
  //     <p *ngIf="currentUser$ | async as user">Logged in as: {{ user.email }}</p>
  //   </div>
  // `
})
export class Dashboard implements OnInit {
  authService = inject(AuthService);
  currentUser$ = this.authService.currentUser$;

  constructor() {}

  ngOnInit(): void {}
}
