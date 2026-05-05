import { Directive, ElementRef, Input, OnInit, OnDestroy, Output, EventEmitter, inject } from '@angular/core';
import flatpickr from 'flatpickr';
import { Instance } from 'flatpickr/dist/types/instance';

@Directive({
  selector: '[appFlatpickr]',
  standalone: true
})
export class FlatpickrDirective implements OnInit, OnDestroy {
  private el = inject(ElementRef);

  @Input() flatpickrOptions: any = {};
  @Input() initialDate?: string | Date;
  @Output() dateChange = new EventEmitter<Date | null>();

  private flatpickrInstance: Instance | null = null;

  ngOnInit(): void {
    const defaultOptions = {
      dateFormat: 'Y-m-d',
      altInput: true,
      altFormat: 'M j, Y',
      allowInput: true,
      onChange: (selectedDates: Date[]) => {
        this.dateChange.emit(selectedDates[0] || null);
      }
    };

    const options = { ...defaultOptions, ...this.flatpickrOptions };

    // Set initial date if provided
    if (this.initialDate) {
      options.defaultDate = this.initialDate;
    }

    this.flatpickrInstance = flatpickr(this.el.nativeElement, options);
  }

  ngOnDestroy(): void {
    if (this.flatpickrInstance) {
      this.flatpickrInstance.destroy();
    }
  }

  public setDate(date: string | Date | null): void {
    if (this.flatpickrInstance) {
      this.flatpickrInstance.setDate(date || '');
    }
  }
}
