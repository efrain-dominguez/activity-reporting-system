import { TemplateRef } from '@angular/core';

export interface DataTableColumn<T = any> {
  key: string;
  label: string;
  sortable?: boolean;
  width?: string;
  cellTemplate?: TemplateRef<any>;
  valueGetter?: (row: T) => any;
}

export interface DataTableAction<T = any> {
  label: string;
  icon?: string;
  variant?: 'default' | 'success' | 'error' | 'warning' | 'info';
  visible?: (row: T) => boolean;
  handler: (row: T) => void;
}

export interface DataTableConfig {
  searchPlaceholder?: string;
  pageSizeOptions?: number[];
  defaultPageSize?: number;
  defaultSortColumn?: string;
  defaultSortDirection?: 'asc' | 'desc';
  showSearch?: boolean;
  showPageSize?: boolean;
  showPagination?: boolean;
}
