import { Injectable } from '@angular/core';
import { RequestAssignment, AssignmentStatus, CreateAssignmentDto, DelegateAssignmentDto, RequestExtensionDto, ReportNoProgressDto } from '../models/assignment.model';

@Injectable({
  providedIn: 'root'
})
export class AssignmentsService {



  // Mock data - replace with actual API calls
  private mockAssignments: RequestAssignment[] = [
    {
      id: 'assign1',
      requestId: 'req1',
      assignedToEntityId: 'entity1', // Finance Department
      assignedToUserId: 'user1',
      assignedByUserId: 'pmo1',
      delegatedFromUserId: undefined,
      status: AssignmentStatus.Pending,
      extensionRequested: false,
      extensionGranted: false,
      createdAt: new Date('2024-01-10'),
      updatedAt: new Date('2024-01-10')
    },
    {
      id: 'assign2',
      requestId: 'req1',
      assignedToEntityId: 'entity2', // HR Department
      assignedToUserId: undefined,
      assignedByUserId: 'pmo1',
      delegatedFromUserId: undefined,
      status: AssignmentStatus.InProgress,
      extensionRequested: false,
      extensionGranted: false,
      submittedAt: new Date('2024-01-20'),
      createdAt: new Date('2024-01-10'),
      updatedAt: new Date('2024-01-20')
    },
    {
      id: 'assign3',
      requestId: 'req1',
      assignedToEntityId: 'entity3', // IT Department
      assignedToUserId: 'user3',
      assignedByUserId: 'pmo1',
      delegatedFromUserId: undefined,
      status: AssignmentStatus.Delegated,
      extensionRequested: false,
      extensionGranted: false,
      createdAt: new Date('2024-01-10'),
      updatedAt: new Date('2024-01-15')
    },
    {
      id: 'assign4',
      requestId: 'req2',
      assignedToEntityId: 'entity1',
      assignedToUserId: undefined,
      assignedByUserId: 'pmo1',
      delegatedFromUserId: undefined,
      status: AssignmentStatus.Overdue,
      extensionRequested: true,
      extensionRequestedDate: new Date('2024-02-20'),
      extensionGranted: false,
      createdAt: new Date('2024-02-01'),
      updatedAt: new Date('2024-02-20')
    }
  ];

  // Get all assignments
  getAssignments(): RequestAssignment[] {
    return this.mockAssignments;
  }

  // Get assignment by ID
  getAssignmentById(id: string): RequestAssignment | undefined {
    return this.mockAssignments.find(a => a.id === id);
  }

  // Get assignments for current user (assigned to me)
  getMyAssignments(userId: string): RequestAssignment[] {
    return this.mockAssignments.filter(a => a.assignedToUserId === userId);
  }

  // Get assignments for current user's entity
  getEntityAssignments(entityId: string): RequestAssignment[] {
    return this.mockAssignments.filter(a => a.assignedToEntityId === entityId);
  }

  // Get assignments delegated by me
  getDelegatedByMe(userId: string): RequestAssignment[] {
    return this.mockAssignments.filter(a => a.delegatedFromUserId === userId);
  }

  // Get assignments for a specific request
  getAssignmentsByRequestId(requestId: string): RequestAssignment[] {
    return this.mockAssignments.filter(a => a.requestId === requestId);
  }

  // Create assignment
  createAssignment(dto: CreateAssignmentDto): RequestAssignment {
    const newAssignment: RequestAssignment = {
      id: `assign${this.mockAssignments.length + 1}`,
      ...dto,
      status: AssignmentStatus.Pending,
      extensionRequested: false,
      extensionGranted: false,
      createdAt: new Date(),
      updatedAt: new Date()
    };
    this.mockAssignments.push(newAssignment);
    return newAssignment;
  }

  // Delegate assignment (creates new assignment)
  delegateAssignment(assignmentId: string, dto: DelegateAssignmentDto, delegatedByUserId: string): RequestAssignment | undefined {
    const originalAssignment = this.getAssignmentById(assignmentId);
    if (!originalAssignment) return undefined;

    // Mark original as delegated
    originalAssignment.status = AssignmentStatus.Delegated;
    originalAssignment.updatedAt = new Date();

    // Create new assignment
    const newAssignment: RequestAssignment = {
      id: `assign${this.mockAssignments.length + 1}`,
      requestId: originalAssignment.requestId,
      assignedToEntityId: dto.assignedToEntityId,
      assignedToUserId: dto.assignedToUserId,
      assignedByUserId: delegatedByUserId,
      delegatedFromUserId: delegatedByUserId,
      status: AssignmentStatus.Pending,
      extensionRequested: false,
      extensionGranted: false,
      createdAt: new Date(),
      updatedAt: new Date()
    };
    this.mockAssignments.push(newAssignment);
    return newAssignment;
  }

  // Update assignment status
  updateAssignmentStatus(assignmentId: string, status: AssignmentStatus): RequestAssignment | undefined {
    const assignment = this.getAssignmentById(assignmentId);
    if (assignment) {
      assignment.status = status;
      assignment.updatedAt = new Date();

      if (status === AssignmentStatus.Completed) {
        assignment.submittedAt = new Date();
      }

      return assignment;
    }
    return undefined;
  }

  // Request extension
  requestExtension(assignmentId: string, dto: RequestExtensionDto): RequestAssignment | undefined {
    const assignment = this.getAssignmentById(assignmentId);
    if (assignment) {
      assignment.extensionRequested = true;
      assignment.extensionRequestedDate = new Date();
      assignment.newDueDate = dto.requestedDueDate;
      assignment.updatedAt = new Date();
      return assignment;
    }
    return undefined;
  }

  // Report no progress
  reportNoProgress(assignmentId: string, dto: ReportNoProgressDto): RequestAssignment | undefined {
    const assignment = this.getAssignmentById(assignmentId);
    if (assignment) {
      assignment.status = AssignmentStatus.NoProgress;
      assignment.noProgressReason = dto.reason;
      assignment.updatedAt = new Date();
      return assignment;
    }
    return undefined;
  }

  // Get assignment statistics for entity
  getEntityStats(entityId: string): {
    total: number;
    pending: number;
    inProgress: number;
    completed: number;
    overdue: number;
    delegated: number;
  } {
    const assignments = this.getEntityAssignments(entityId);
    return {
      total: assignments.length,
      pending: assignments.filter(a => a.status === AssignmentStatus.Pending).length,
      inProgress: assignments.filter(a => a.status === AssignmentStatus.InProgress).length,
      completed: assignments.filter(a => a.status === AssignmentStatus.Completed).length,
      overdue: assignments.filter(a => a.status === AssignmentStatus.Overdue).length,
      delegated: assignments.filter(a => a.status === AssignmentStatus.Delegated).length
    };
  }
}
