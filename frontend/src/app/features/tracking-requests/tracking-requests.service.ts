import { Injectable } from '@angular/core';
import { TrackingRequest, RequestStatus, CreateTrackingRequestDto, UpdateTrackingRequestDto } from '../../shared/models/tracking-request.model';
import { Frequency } from '../../shared/models/frequency.model';

@Injectable({
  providedIn: 'root'
})
export class TrackingRequestsService {

  // Mock data
 private mockRequests: TrackingRequest[] = [
  {
    id: 'req1',
    title: 'Q1 Financial Compliance Review',
    description: 'Track quarterly financial compliance activities across all departments',
    goalType: 'Financial',
    createdByUserId: 'user1',
    targetEntityIds: ['entity1', 'entity2', 'entity3'],
    startDate: new Date('2024-01-01'),
    dueDate: new Date('2024-03-31'),
    frequency: Frequency.Quarterly,
    status: RequestStatus.InProgress,
    isRecurring: true,
    createdAt: new Date('2023-12-15'),
    updatedAt: new Date('2024-01-10')
  },
  {
    id: 'req2',
    title: 'Employee Training Completion',
    description: 'Monitor completion of mandatory safety training for all staff',
    goalType: 'HR',
    createdByUserId: 'user1',
    targetEntityIds: ['entity2', 'entity4'],
    startDate: new Date('2024-02-01'),
    dueDate: new Date('2024-02-28'),
    frequency: Frequency.Monthly,
    status: RequestStatus.Pending,
    isRecurring: true,
    createdAt: new Date('2024-01-20'),
    updatedAt: new Date('2024-01-20')
  },
  {
    id: 'req3',
    title: 'IT Security Audit',
    description: 'Conduct comprehensive security audit of all IT systems',
    goalType: 'IT',
    createdByUserId: 'user1',
    targetEntityIds: ['entity3'],
    startDate: new Date('2024-01-15'),
    dueDate: new Date('2024-01-10'),
    status: RequestStatus.Overdue,
    isRecurring: false,
    createdAt: new Date('2024-01-05'),
    updatedAt: new Date('2024-01-05')
  },
  {
    id: 'req4',
    title: 'Marketing Campaign Analysis',
    description: 'Analyze effectiveness of Q4 marketing campaigns',
    goalType: 'Marketing',
    createdByUserId: 'user1',
    targetEntityIds: ['entity4'],
    startDate: new Date('2024-01-01'),
    dueDate: new Date('2024-01-31'),
    status: RequestStatus.Completed,
    isRecurring: false,
    createdAt: new Date('2023-12-20'),
    updatedAt: new Date('2024-01-25')
  },
  {
    id: 'req5',
    title: 'Quarterly Operations Review',
    description: 'Review operational efficiency and identify improvement areas',
    goalType: 'Operations',
    createdByUserId: 'user1',
    targetEntityIds: ['entity1', 'entity5'],
    startDate: new Date('2024-03-01'),
    dueDate: new Date('2024-03-31'),
    frequency: Frequency.Quarterly,
    status: RequestStatus.Draft,
    isRecurring: true,
    createdAt: new Date('2024-02-15'),
    updatedAt: new Date('2024-02-15')
  },
  {
    id: 'req6',
    title: 'Customer Satisfaction Survey',
    description: 'Conduct monthly customer satisfaction surveys across all touchpoints',
    goalType: 'Marketing',
    createdByUserId: 'user1',
    targetEntityIds: ['entity1', 'entity4'],
    startDate: new Date('2024-03-01'),
    dueDate: new Date('2024-03-31'),
    frequency: Frequency.Monthly,
    status: RequestStatus.Pending,
    isRecurring: true,
    createdAt: new Date('2024-02-20'),
    updatedAt: new Date('2024-02-20')
  },
  {
    id: 'req7',
    title: 'Annual Budget Planning',
    description: 'Coordinate annual budget planning across all departments',
    goalType: 'Financial',
    createdByUserId: 'user1',
    targetEntityIds: ['entity1', 'entity2', 'entity3', 'entity4', 'entity5'],
    startDate: new Date('2024-01-01'),
    dueDate: new Date('2024-12-31'),
    frequency: Frequency.Yearly,
    status: RequestStatus.InProgress,
    isRecurring: false,
    createdAt: new Date('2024-01-01'),
    updatedAt: new Date('2024-01-15')
  },
  {
    id: 'req8',
    title: 'Weekly Team Standup Reports',
    description: 'Track weekly standup reports from all team leads',
    goalType: 'Operations',
    createdByUserId: 'user1',
    targetEntityIds: ['entity2', 'entity3', 'entity5'],
    startDate: new Date('2024-01-01'),
    dueDate: new Date('2024-12-31'),
    frequency: Frequency.Weekly,
    status: RequestStatus.InProgress,
    isRecurring: true,
    createdAt: new Date('2024-01-01'),
    updatedAt: new Date('2024-01-10')
  },
  {
    id: 'req9',
    title: 'Workplace Safety Inspection',
    description: 'Monthly safety inspections of all facilities',
    goalType: 'Safety',
    createdByUserId: 'user1',
    targetEntityIds: ['entity1', 'entity2', 'entity3', 'entity4', 'entity5'],
    startDate: new Date('2024-02-01'),
    dueDate: new Date('2024-02-29'),
    frequency: Frequency.Monthly,
    status: RequestStatus.Completed,
    isRecurring: true,
    createdAt: new Date('2024-01-15'),
    updatedAt: new Date('2024-02-28')
  },
  {
    id: 'req10',
    title: 'Compliance Documentation Update',
    description: 'Update compliance documentation for regulatory changes',
    goalType: 'Compliance',
    createdByUserId: 'user1',
    targetEntityIds: ['entity1', 'entity3'],
    startDate: new Date('2024-03-01'),
    dueDate: new Date('2024-03-15'),
    status: RequestStatus.Draft,
    isRecurring: false,
    createdAt: new Date('2024-02-25'),
    updatedAt: new Date('2024-02-25')
  },
  {
    id: 'req11',
    title: 'IT Infrastructure Monitoring',
    description: 'Daily monitoring of IT infrastructure health and performance',
    goalType: 'IT',
    createdByUserId: 'user1',
    targetEntityIds: ['entity3'],
    startDate: new Date('2024-01-01'),
    dueDate: new Date('2024-12-31'),
    frequency: Frequency.Daily,
    status: RequestStatus.InProgress,
    isRecurring: true,
    createdAt: new Date('2024-01-01'),
    updatedAt: new Date('2024-01-20')
  },
  {
    id: 'req12',
    title: 'Employee Performance Reviews',
    description: 'Quarterly performance reviews for all departments',
    goalType: 'HR',
    createdByUserId: 'user1',
    targetEntityIds: ['entity2', 'entity4', 'entity5'],
    startDate: new Date('2024-04-01'),
    dueDate: new Date('2024-04-30'),
    frequency: Frequency.Quarterly,
    status: RequestStatus.Pending,
    isRecurring: true,
    createdAt: new Date('2024-03-15'),
    updatedAt: new Date('2024-03-15')
  }
];

  // Get all tracking requests (returns array directly for mock data)
  getTrackingRequests(): TrackingRequest[] {
    return this.mockRequests;
  }

  // Get single tracking request by ID
  getTrackingRequestById(id: string): TrackingRequest | undefined {
    return this.mockRequests.find(req => req.id === id);
  }

  // Create new tracking request
  createTrackingRequest(dto: CreateTrackingRequestDto): TrackingRequest {
    const newRequest: TrackingRequest = {
      id: 'req' + (this.mockRequests.length + 1),
      ...dto,
      createdByUserId: 'currentUser',
      status: RequestStatus.Draft,
      createdAt: new Date(),
      updatedAt: new Date()
    };
    this.mockRequests.push(newRequest);
    return newRequest;
  }

  // Update tracking request
  updateTrackingRequest(id: string, dto: UpdateTrackingRequestDto): TrackingRequest | undefined {
    const index = this.mockRequests.findIndex(req => req.id === id);
    if (index !== -1) {
      this.mockRequests[index] = {
        ...this.mockRequests[index],
        ...dto,
        updatedAt: new Date()
      };
      return this.mockRequests[index];
    }
    return undefined;
  }

  // Submit tracking request
  submitTrackingRequest(id: string): TrackingRequest | undefined {
    const request = this.mockRequests.find(req => req.id === id);
    if (request && request.status === RequestStatus.Draft) {
      request.status = RequestStatus.Pending;
      request.updatedAt = new Date();
      return request;
    }
    return undefined;
  }

  // Delete tracking request
  deleteTrackingRequest(id: string): boolean {
    const index = this.mockRequests.findIndex(req => req.id === id);
    if (index !== -1) {
      this.mockRequests.splice(index, 1);
      return true;
    }
    return false;
  }
}
