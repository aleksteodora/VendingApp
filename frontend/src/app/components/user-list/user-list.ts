import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService, UserModel } from '../../services/user';

@Component({
  selector: 'app-user-list',
  imports: [CommonModule, FormsModule],
  templateUrl: './user-list.html',
  styleUrl: './user-list.css'
})
export class UserList implements OnInit {
  users: UserModel[] = [];
  isEditing = false;
  errorMessage = '';
  successMessage = '';

  pageNumber = 1;
  pageSize = 20;
  totalCount = 0;
  totalPages = 0;

  currentUser: UserModel = {
    fullName: '',
    address: '',
    phoneNumber: '',
    meterSerialNumber: ''
  };

  constructor(private userService: UserService, private cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.userService.getAll(this.pageNumber, this.pageSize).subscribe({
      next: (result) => {
        this.users = result.items;
        this.totalCount = result.totalCount;
        this.totalPages = result.totalPages;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading users:', err);
        this.errorMessage = 'Could not load the user list. Check if the server is running.';
        this.cdr.detectChanges();
      }
    });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) {
      return;
    }
    this.pageNumber = page;
    this.loadUsers();
  }

  nextPage(): void {
    this.goToPage(this.pageNumber + 1);
  }

  previousPage(): void {
    this.goToPage(this.pageNumber - 1);
  }

  onSubmit(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.currentUser.fullName?.trim() || !this.currentUser.address?.trim() ||
        !this.currentUser.phoneNumber?.trim() || !this.currentUser.meterSerialNumber?.trim()) {
      this.errorMessage = 'All fields are required. Please check your input and try again.';
      return;
    }

    if (this.isEditing && this.currentUser.id) {
      this.userService.update(this.currentUser.id, this.currentUser).subscribe({
        next: () => {
          this.loadUsers();
          this.successMessage = 'User updated successfully.';
          this.resetForm();
          this.cdr.detectChanges();
        },
        error: (err) => this.handleError(err)
      });
    } else {
      this.userService.create(this.currentUser).subscribe({
        next: () => {
          this.pageNumber = 1;
          this.loadUsers();
          this.successMessage = 'User added successfully.';
          this.resetForm();
          this.cdr.detectChanges();
        },
        error: (err) => this.handleError(err)
      });
    }
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

  editUser(user: UserModel): void {
    this.currentUser = { ...user };
    this.isEditing = true;
    this.errorMessage = '';
    this.successMessage = '';
    this.cdr.detectChanges();
  }

  deleteUser(id: number): void {
    if (confirm('Are you sure you want to delete this user?')) {
      this.userService.delete(id).subscribe({
        next: () => {
          this.loadUsers();
          this.successMessage = 'User deleted.';
          this.cdr.detectChanges();
        },
        error: (err) => this.handleError(err)
      });
    }
  }

  resetForm(): void {
    this.currentUser = {
      fullName: '',
      address: '',
      phoneNumber: '',
      meterSerialNumber: ''
    };
    this.isEditing = false;
  }
}