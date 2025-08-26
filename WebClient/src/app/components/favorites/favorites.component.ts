import { Component, inject } from '@angular/core';
import { GithubRepo } from '../../models/githubrepo';
import { SearchService } from '../../services/search.service';
import { CommonModule } from '@angular/common';
import { ResponseUser } from '../../models/responseuser';
import { UserService } from '../../services/user.service';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-favorites',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './favorites.component.html',
  styleUrl: './favorites.component.css'
})
export class FavoritesComponent {

  bookmarks: GithubRepo[] = [];
  currentUser: ResponseUser | null = null;
  private _userService = inject(UserService);
  private readonly destroy$ = new Subject()

  constructor(private userSearch: SearchService) {}

  ngOnInit(): void {
    this.getCurrentUser();
    this.getBookmarks()
  }

  getCurrentUser(): void {
    this._userService.currentUser$.pipe(
      takeUntil(this.destroy$) // Automatically unsubscribe on component destruction
    ).subscribe(user => {
      this.currentUser = user;
      console.log('User state updated in SearchComponent:', this.currentUser?.id);
    });
  }

  getBookmarks(): void {
    if (this.currentUser) {
      this.userSearch.getBookmarks(this.currentUser.id).subscribe({
        next: (bookmarks) => {
          this.bookmarks = bookmarks; // Load bookmarks
        },
        error: (err) => {
          console.error('Error fetching bookmarks:', err);
        }
      });
    }
  }
  ngOnDestroy(): void {
    this.destroy$.next(true);
    this.destroy$.complete();
  }
}
