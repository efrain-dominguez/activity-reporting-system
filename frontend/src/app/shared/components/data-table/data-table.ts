import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges, ContentChildren, QueryList, TemplateRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataTableColumn, DataTableAction, DataTableConfig } from './data-table.models';
import { TooltipDirective } from '../../directives/tooltip';

@Component({
  selector: 'app-data-table',
  standalone: true,
  imports: [CommonModule, FormsModule, TooltipDirective],
  templateUrl: './data-table.html',
  styleUrls: ['./data-table.css']
})
export class DataTable<T = any> implements OnInit, OnChanges {
  @Input() data: T[] = [];
  @Input() columns: DataTableColumn<T>[] = [];
  @Input() actions: DataTableAction<T>[] = [];
  @Input() config: DataTableConfig = {};

  @Output() rowClick = new EventEmitter<T>();

  // Internal state
  filteredData: T[] = [];
  paginatedData: T[] = [];

  searchTerm = '';
  sortColumn: string = '';
  sortDirection: 'asc' | 'desc' = 'asc';

  currentPage = 1;
  pageSize = 10;
  totalPages = 0;

  // Expose Math to template
  Math = Math;

  ngOnInit(): void {
    this.applyDefaults();
    this.applyFilters();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['data']) {
      this.applyFilters();
    }
  }

  private applyDefaults(): void {
    this.pageSize = this.config.defaultPageSize || 10;
    this.sortColumn = this.config.defaultSortColumn || '';
    this.sortDirection = this.config.defaultSortDirection || 'asc';
  }

  get pageSizeOptions(): number[] {
    return this.config.pageSizeOptions || [5, 10, 25, 50];
  }

  get showSearch(): boolean {
    return this.config.showSearch !== false;
  }

  get showPageSize(): boolean {
    return this.config.showPageSize !== false;
  }

  get showPagination(): boolean {
    return this.config.showPagination !== false;
  }

  get searchPlaceholder(): string {
    return this.config.searchPlaceholder || 'Search...';
  }

  // Search
  onSearchChange(): void {
    this.currentPage = 1;
    this.applyFilters();
  }

  // Sorting
  onSort(column: DataTableColumn<T>): void {
    if (!column.sortable) return;

    if (this.sortColumn === column.key) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = column.key;
      this.sortDirection = 'asc';
    }
    this.applyFilters();
  }

  getSortIcon(column: DataTableColumn<T>): string {
    if (!column.sortable || this.sortColumn !== column.key) {
      return 'M7 10l5 5 5-5H7z';
    }
    return this.sortDirection === 'asc'
      ? 'M7 14l5-5 5 5H7z'
      : 'M7 10l5 5 5-5H7z';
  }

  // Pagination
  onPageChange(page: number): void {
    this.currentPage = page;
    this.applyPagination();
  }

  onPageSizeChange(): void {
    this.currentPage = 1;
    this.applyFilters();
  }

  getPageNumbers(): number[] {
    const pages: number[] = [];
    const maxVisible = 5;

    if (this.totalPages <= maxVisible) {
      for (let i = 1; i <= this.totalPages; i++) {
        pages.push(i);
      }
    } else {
      if (this.currentPage <= 3) {
        pages.push(1, 2, 3, 4, -1, this.totalPages);
      } else if (this.currentPage >= this.totalPages - 2) {
        pages.push(1, -1, this.totalPages - 3, this.totalPages - 2, this.totalPages - 1, this.totalPages);
      } else {
        pages.push(1, -1, this.currentPage - 1, this.currentPage, this.currentPage + 1, -1, this.totalPages);
      }
    }

    return pages;
  }

  // Filter/Sort/Paginate
  applyFilters(): void {
    // 1. Search filter
    if (this.searchTerm.trim()) {
      const term = this.searchTerm.toLowerCase();
      this.filteredData = this.data.filter(item => {
        return this.columns.some(col => {
          const value = this.getCellValue(item, col);
          return value?.toString().toLowerCase().includes(term);
        });
      });
    } else {
      this.filteredData = [...this.data];
    }

    // 2. Sort
    if (this.sortColumn) {
      const column = this.columns.find(col => col.key === this.sortColumn);
      if (column) {
        this.filteredData.sort((a, b) => {
          const aVal = this.getCellValue(a, column);
          const bVal = this.getCellValue(b, column);

          let comparison = 0;
          if (aVal < bVal) comparison = -1;
          if (aVal > bVal) comparison = 1;

          return this.sortDirection === 'asc' ? comparison : -comparison;
        });
      }
    }

    // 3. Pagination
    this.totalPages = Math.ceil(this.filteredData.length / this.pageSize);
    this.applyPagination();
  }

  applyPagination(): void {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.paginatedData = this.filteredData.slice(startIndex, endIndex);
  }

  // Cell value extraction
  getCellValue(row: T, column: DataTableColumn<T>): any {
    if (column.valueGetter) {
      return column.valueGetter(row);
    }
    return (row as any)[column.key];
  }

  // Actions
  isActionVisible(action: DataTableAction<T>, row: T): boolean {
    return action.visible ? action.visible(row) : true;
  }

  getActionClass(action: DataTableAction<T>): string {
    const baseClass = 'btn size-8 p-0';

    switch (action.variant) {
      case 'error':
        return `${baseClass} hover:bg-error/20 focus:bg-error/20 active:bg-error/25`;
      case 'success':
        return `${baseClass} hover:bg-success/20 focus:bg-success/20 active:bg-success/25`;
      case 'warning':
        return `${baseClass} hover:bg-warning/20 focus:bg-warning/20 active:bg-warning/25`;
      case 'info':
        return `${baseClass} hover:bg-info/20 focus:bg-info/20 active:bg-info/25`;
      default:
        return `${baseClass} hover:bg-slate-300/20 focus:bg-slate-300/20 active:bg-slate-300/25 dark:hover:bg-navy-300/20 dark:focus:bg-navy-300/20 dark:active:bg-navy-300/25`;
    }
  }

  getActionIconClass(action: DataTableAction<T>): string {
    switch (action.variant) {
      case 'error':
        return 'text-error';
      case 'success':
        return 'text-success';
      case 'warning':
        return 'text-warning';
      case 'info':
        return 'text-info';
      default:
        return '';
    }
  }

  onRowClick(row: T): void {
    this.rowClick.emit(row);
  }
}
