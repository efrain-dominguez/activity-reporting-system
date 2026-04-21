import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SidebarService {
  toggle(): void {
    document.body.classList.toggle('is-sidebar-open');
  }
}
