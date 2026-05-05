import { Directive, ElementRef, Input, OnInit, OnDestroy, inject } from '@angular/core';
import tippy, { Instance, Placement, Props } from 'tippy.js';

@Directive({
  selector: '[appTooltip]',
  standalone: true
})
export class TooltipDirective implements OnInit, OnDestroy {
  private el = inject(ElementRef);

  @Input() appTooltip: string = '';
  @Input() tooltipPlacement: Placement = 'top';
  @Input() tooltipTheme: 'dark' | 'light' = 'dark';

  private tippyInstance?: Instance<Props>;

  ngOnInit(): void {
    if (this.appTooltip && this.el.nativeElement) {
      // Create tooltip and extract first instance
      const result = tippy(this.el.nativeElement, {
        content: this.appTooltip,
        placement: this.tooltipPlacement,
        theme: this.tooltipTheme,
        arrow: true,
        animation: 'scale',
        duration: [200, 150]
      });

      // tippy always returns an array, get the first instance
      if (Array.isArray(result) && result.length > 0) {
        this.tippyInstance = result[0];
      }
    }
  }

  ngOnDestroy(): void {
    this.tippyInstance?.destroy();
  }
}
