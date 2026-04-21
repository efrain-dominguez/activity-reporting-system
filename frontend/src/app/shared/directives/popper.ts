import { Directive, ElementRef, Input, OnDestroy, inject, AfterViewInit } from '@angular/core';
import { createPopper, Instance, Placement } from '@popperjs/core';

@Directive({
  selector: '[appPopper]',
  standalone: true,
  host: {
    '(click)': 'toggle($event)'
  }
})
export class PopperDirective implements AfterViewInit, OnDestroy {
  private el = inject(ElementRef);
  
  @Input() appPopper!: string; // Target element selector (e.g., '#notification-box')
  @Input() popperPlacement: Placement = 'bottom-end';
  @Input() popperOffset: [number, number] = [0, 12];
  
  private popperInstance: Instance | null = null;
  private targetElement: HTMLElement | null = null;
  private isOpen = false;

  ngAfterViewInit(): void {
    this.targetElement = document.querySelector(this.appPopper);
    
    if (this.targetElement) {
      this.popperInstance = createPopper(this.el.nativeElement, this.targetElement, {
        placement: this.popperPlacement,
        modifiers: [
          {
            name: 'offset',
            options: {
              offset: this.popperOffset,
            },
          },
        ],
      });
    }
  }

  toggle(event: Event): void {
    event.stopPropagation();
    
    if (!this.targetElement) return;

    this.isOpen = !this.isOpen;

    if (this.isOpen) {
      this.targetElement.classList.remove('hidden');
      this.popperInstance?.update();
      
      // Close on outside click
      setTimeout(() => {
        document.addEventListener('click', this.closePopper);
      }, 0);
    } else {
      this.closePopper();
    }
  }

  private closePopper = (): void => {
    if (this.targetElement) {
      this.targetElement.classList.add('hidden');
      this.isOpen = false;
      document.removeEventListener('click', this.closePopper);
    }
  }

  ngOnDestroy(): void {
    this.popperInstance?.destroy();
    document.removeEventListener('click', this.closePopper);
  }
}