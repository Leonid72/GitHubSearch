import { Component, ElementRef, inject, signal, ViewChild } from '@angular/core';
import { UserService } from '../../services/user.service';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { ResponseUser } from '../../models/responseuser';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { finalize,take,} from 'rxjs';

declare const bootstrap: any; 
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginData = { UserName: '', Password: '' };
  newUserData = { UserName: '', Password: '' };
  //Get modal element reference
  @ViewChild('createUserModalEl') createUserModalEl!: ElementRef<HTMLDivElement>;
  private userService = inject(UserService);
  private router = inject(Router);
  private activatedRoute = inject(ActivatedRoute);
  private toastr = inject(ToastrService);

  private _loading = signal(false);
  get loading() {return this._loading; }

  returnUrl!: string;

  constructor() {
     this.returnUrl = this.activatedRoute.snapshot.queryParams['returnUrl'] || '/search'
  }
 

  onLogin() {
  this.loading.set(true); // display progress bar
  this.userService.login(this.loginData)
    .pipe(take(1), finalize(() => this.loading.set(false)))
    .subscribe({
      next: (response: ResponseUser) => {
        console.log('Login successful:', response);
        this.toastr.success('Login successful!', 'Success');
        this.router.navigateByUrl(this.returnUrl);
      },
      error: (err) => {
        console.error('Login failed:', err);
        this.toastr.error('Login failed. Please try again.', 'Error');
      }
    });
  }

  onCreateUser() {
    this.userService.createUser(this.newUserData)
    .pipe(take(1), finalize(() => this.loading.set(false)))
    .subscribe({
      next: (response: ResponseUser) => {
        console.log('User created successfully!:', response);
        this.toastr.success('User created successfully!', 'Success');
        this.closeCreateUserModal(); 
        this.router.navigateByUrl(this.returnUrl);
      },
      error: (err) => {
        console.error('User creation failed:', err);
        this.toastr.error('User creation failed. Please try again.', 'Error');
      }
    });
  }

  /**
 * Closes the "Create User" Bootstrap 5 modal.
 * Uses the official Bootstrap Modal API, then defensively cleans up any
 * leftovers (backdrop / body class) in case the modal was left in a bad state.
 */
  private closeCreateUserModal() {
    const el = this.createUserModalEl?.nativeElement;
    if (!el) return;

    //Get an existing Bootstrap Modal instance (created by data attributes),
    //    or create one so we can call `hide()`.
    const instance =
      bootstrap.Modal.getInstance(el) ?? new bootstrap.Modal(el);

    instance.hide();

    document.body.classList.remove('modal-open');
    document.querySelectorAll('.modal-backdrop').forEach(b => b.remove());
  }
}