import { Component } from '@angular/core';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {

  constructor(private userService: UserService, private router: Router) {}

  logout(): void {
    this.userService.logout(); // Clear user data
    this.router.navigate(['/login']); // Redirect to login page
  }
}
