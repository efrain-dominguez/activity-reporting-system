export interface Entity {
  id: string;
  name: string;
  code: string;
  description: string;
  parentEntityId?: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}
