import { Component, AfterViewInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SidebarComponent } from '../sidebar/sidebar';
import { HeaderComponent } from '../header/header';
import { RightSidebarComponent } from '../right-sidebar/right-sidebar';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    SidebarComponent,
    HeaderComponent,
    RightSidebarComponent
  ],
  templateUrl: './main-layout.html',
  styleUrls: ['./main-layout.css']
})
export class MainLayoutComponent implements AfterViewInit, OnDestroy {
  private appMountedListener: any;

  ngAfterViewInit(): void {
    // Listen for LineOne's "app:mounted" event
    this.appMountedListener = () => {
      console.log('✅ LineOne App mounted and initialized');
    };

    window.addEventListener('app:mounted', this.appMountedListener);
  }

  ngOnDestroy(): void {
    if (this.appMountedListener) {
      window.removeEventListener('app:mounted', this.appMountedListener);
    }
  }
}
