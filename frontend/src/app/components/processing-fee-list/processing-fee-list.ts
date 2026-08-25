import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProcessingFeeService, ProcessingFeeModel } from '../../services/processing-fee';

@Component({
  selector: 'app-processing-fee-list',
  imports: [CommonModule, FormsModule],
  templateUrl: './processing-fee-list.html',
  styleUrl: './processing-fee-list.css'
})
export class ProcessingFeeList implements OnInit {
  activeFee: ProcessingFeeModel | null = null;
  history: ProcessingFeeModel[] = [];
  errorMessage = '';
  successMessage = '';

  fixedAmount: number | null = null;
  percentageRate: number | null = null;

  constructor(private feeService: ProcessingFeeService, private cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.loadActive();
    this.loadHistory();
  }

  loadActive(): void {
    this.feeService.getActive().subscribe({
      next: (data) => {
        this.activeFee = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading active fee:', err);
        this.errorMessage = 'Could not load the current processing fee.';
        this.cdr.detectChanges();
      }
    });
  }

  loadHistory(): void {
    this.feeService.getHistory().subscribe({
      next: (data) => {
        this.history = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading fee history:', err);
        this.cdr.detectChanges();
      }
    });
  }

  onSubmit(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (this.fixedAmount === null || this.fixedAmount < 0) {
      this.errorMessage = 'Fixed amount must be a non-negative number.';
      this.cdr.detectChanges();
      return;
    }

    if (this.percentageRate === null || this.percentageRate < 0 || this.percentageRate > 1) {
      this.errorMessage = 'Percentage rate must be between 0 and 1.';
      this.cdr.detectChanges();
      return;
    }

    this.feeService.change({
      fixedAmount: this.fixedAmount,
      percentageRate: this.percentageRate
    }).subscribe({
      next: () => {
        this.successMessage = 'Processing fee updated successfully.';
        this.fixedAmount = null;
        this.percentageRate = null;
        this.loadActive();
        this.loadHistory();
        this.cdr.detectChanges();
      },
      error: (err) => this.handleError(err)
    });
  }

  private handleError(err: any): void {
    console.error('Request failed:', err);

    if (err.status === 0) {
      this.errorMessage = 'Could not connect to the server.';
    } else if (err.error?.message) {
      this.errorMessage = err.error.message;
    } else if (err.error?.errors) {
      const firstError = Object.values(err.error.errors)[0] as string[];
      this.errorMessage = firstError?.[0] || 'Please check your input and try again.';
    } else {
      this.errorMessage = 'Something went wrong. Please try again.';
    }

    this.cdr.detectChanges();
  }
}