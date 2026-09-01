import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { AdminManagementService, AdminManagementModel } from '../../services/admin-management';

@Component({
  selector: 'app-admin-list',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './admin-list.html',
  styleUrl: './admin-list.css'
})
export class AdminList implements OnInit {
  admins: AdminManagementModel[] = [];
  isEditing = false;
  editingId: number | null = null;
  errorMessage = '';
  successMessage = '';
  showPassword = false;

  pageNumber = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 0;

  adminForm: FormGroup;

  constructor(
    private adminService: AdminManagementService,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {
    this.adminForm = this.fb.group({
      fullName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  ngOnInit(): void {
    this.loadAdmins();
  }

  loadAdmins(): void {
    this.adminService.getAll(this.pageNumber, this.pageSize).subscribe({
      next: (result) => {
        this.admins = result.items;
        this.totalCount = result.totalCount;
        this.totalPages = result.totalPages;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading admins:', err);
        this.errorMessage = 'Could not load the admin list.';
        this.cdr.detectChanges();
      }
    });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) {
      return;
    }
    this.pageNumber = page;
    this.loadAdmins();
  }

  nextPage(): void {
    this.goToPage(this.pageNumber + 1);
  }

  previousPage(): void {
    this.goToPage(this.pageNumber - 1);
  }

  get fullName() { return this.adminForm.get('fullName'); }
  get email() { return this.adminForm.get('email'); }
  get password() { return this.adminForm.get('password'); }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  onSubmit(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (this.adminForm.invalid) {
      this.adminForm.markAllAsTouched();
      this.errorMessage = 'Please check the form for errors.';
      this.cdr.detectChanges();
      return;
    }

    const formValue = { ...this.adminForm.value };
    if (this.isEditing && !formValue.password) {
      delete formValue.password;
    }

    if (this.isEditing && this.editingId) {
      this.adminService.update(this.editingId, formValue).subscribe({
        next: () => {
          this.loadAdmins();
          this.successMessage = 'Admin updated successfully.';
          this.resetForm();
          this.cdr.detectChanges();
        },
        error: (err) => this.handleError(err)
      });
    } else {
      this.pageNumber = 1;
      this.adminService.create(formValue).subscribe({
        next: () => {
          this.loadAdmins();
          this.successMessage = 'Admin added successfully.';
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

  editAdmin(admin: AdminManagementModel): void {
    this.isEditing = true;
    this.editingId = admin.id ?? null;

    this.adminForm.get('password')?.clearValidators();
    this.adminForm.get('password')?.setValidators([Validators.minLength(6)]);
    this.adminForm.get('password')?.updateValueAndValidity();

    this.adminForm.patchValue({
      fullName: admin.fullName,
      email: admin.email,
      password: ''
    });
    this.errorMessage = '';
    this.successMessage = '';
    this.cdr.detectChanges();
  }

  deleteAdmin(id: number): void {
    if (confirm('Are you sure you want to delete this admin?')) {
      this.adminService.delete(id).subscribe({
        next: () => {
          this.loadAdmins();
          this.successMessage = 'Admin deleted.';
          this.cdr.detectChanges();
        },
        error: (err) => this.handleError(err)
      });
    }
  }

  resetForm(): void {
    this.adminForm.get('password')?.clearValidators();
    this.adminForm.get('password')?.setValidators([Validators.required, Validators.minLength(6)]);
    this.adminForm.get('password')?.updateValueAndValidity();

    this.adminForm.reset();
    this.isEditing = false;
    this.editingId = null;
  }
}