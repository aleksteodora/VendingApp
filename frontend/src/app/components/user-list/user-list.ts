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
    this.userService.getAll().subscribe({
      next: (data) => {
        this.users = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading users:', err);
        this.errorMessage = 'Lista korisnika nije mogla da se učita. Proverite da li je server pokrenut.';
        this.cdr.detectChanges();
      }
    });
  }

  onSubmit(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.currentUser.fullName?.trim() || !this.currentUser.address?.trim() ||
        !this.currentUser.phoneNumber?.trim() || !this.currentUser.meterSerialNumber?.trim()) {
      this.errorMessage = 'Sva polja su obavezna. Proverite unos i pokušajte ponovo.';
      return;
    }

    if (this.isEditing && this.currentUser.id) {
      this.userService.update(this.currentUser.id, this.currentUser).subscribe({
        next: () => {
          this.loadUsers();
          this.successMessage = 'Korisnik je uspešno izmenjen.';
          this.resetForm();
          this.cdr.detectChanges();
        },
        error: (err) => this.handleError(err)
      });
    } else {
      this.userService.create(this.currentUser).subscribe({
        next: () => {
          this.loadUsers();
          this.successMessage = 'Korisnik je uspešno dodat.';
          this.resetForm();
          this.cdr.detectChanges();
        },
        error: (err) => this.handleError(err)
      });
    }
  }

  private handleError(err: any): void {
    console.error('Request failed:', err);
    if (err.status === 400) {
      this.errorMessage = 'Proverite da li su sva polja ispravno popunjena.';
    } else if (err.status === 0) {
      this.errorMessage = 'Nije moguće povezati se sa serverom.';
    } else {
      this.errorMessage = 'Došlo je do greške. Pokušajte ponovo.';
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
    if (confirm('Da li ste sigurni da želite da obrišete ovog korisnika?')) {
      this.userService.delete(id).subscribe({
        next: () => {
          this.loadUsers();
          this.successMessage = 'Korisnik je obrisan.';
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