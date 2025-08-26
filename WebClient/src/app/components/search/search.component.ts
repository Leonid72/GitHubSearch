import { Component, inject, OnInit, signal } from '@angular/core';
import { finalize, of, Subject,takeUntil } from 'rxjs';
import { GithubRepo } from '../../models/githubrepo';
import { SearchService } from '../../services/search.service';
import { CommonModule } from '@angular/common';
import { SearchParams } from '../../models/searchparams';
import { FormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { UserService } from '../../services/user.service';
import { ResponseUser } from '../../models/responseuser';
import { RouterLink } from '@angular/router';

/**
 * SearchComponent is responsible for displaying a gallery of GitHub repositories
 * and managing bookmarks (favorites) for the logged-in user.
 */
@Component({
  selector: 'app-search',
  imports: [CommonModule,FormsModule, RouterLink],
  standalone: true,
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit {

  // Using Angular's signal for state management
  private _repositories = signal<GithubRepo[]>([]);
  readonly repositories = this._repositories.asReadonly();

  private _search = inject(SearchService);
  private _userService = inject(UserService);

  currentUser: ResponseUser | null = null;

  private _bookmarks = signal<GithubRepo[]>([]);
  readonly bookmarks = this._bookmarks.asReadonly();

  private _toastr = inject(ToastrService);
  private readonly destroy$ = new Subject()

  private _loading = signal(false);
  readonly loading = this._loading.asReadonly();
  error = signal<string | null>(null);
  params: SearchParams = { keywords: 'test', page: 1, perPage: 9 };
  favorites: any[] = [];


  constructor() {}

  /**
   * Lifecycle hook that runs when the component is initialized.
   * Fetches repositories and loads bookmarks for the current user.
   */
  ngOnInit(): void {
    this.fetchRepositories();
    this.getCurrentUser();
  }

   /**
   * Fetches repositories from the GitHub API based on search parameters.
   */
fetchRepositories(): void {
  if (this.loading()) return; //prevent multiple submissions
  this._loading.set(true); // display progress bar
  this._search.searchRepositories(this.params)
    .pipe(takeUntil(this.destroy$), finalize(() => this._loading.set(false)))
    .subscribe({
      next: (repositories) => {
        this._repositories.set(repositories);
      },
      error: (error) => {
        this._toastr.error('Error fetching repositories:', error);
        return of([]); // Return an empty array in case of error
      }
    });
}

// stable identity for *ngFor
trackByName = (_: number, repo: { name: string }) => repo.name;

 /**
   * Toggles the bookmark status of a repository.
   * If the repository is already bookmarked, it removes it from bookmarks.
   * Otherwise, it adds the repository to bookmarks.
   * 
   * @param repo - The repository to toggle bookmark status for
   */
toggleBookmark(repo: GithubRepo): void {
  if (this.isBookmarked(repo)) {
    // Remove bookmark
    this._search.removeBookmark(repo, this.currentUser!.id).subscribe({
      next: () => {
        this._bookmarks.update((bookmark)=> bookmark.filter((b) => b.name !== repo.name));
        this._toastr.info('Removed from bookmarks:', repo.name);
      },
      error: (err) => {
        console.error('Error removing bookmark:', err);
      }
    });
  } else {
    // Add bookmark
    this._search.addToBookmarks(repo, this.currentUser!.id).subscribe({
      next: (response) => {
        this._bookmarks.update((b) => [...b, repo]);
        this._toastr.info('Added to bookmarks:', repo.name);
      },
      error: (err) => {
        console.error('Error adding bookmark:', err);
      }
    });
  }
}
isBookmarked(repo: GithubRepo): boolean {
  return this._bookmarks().some((b) => b.name === repo.name);
}


getCurrentUser(): void {
  this._userService.currentUser$.pipe(
    takeUntil(this.destroy$) // Automatically unsubscribe on component destruction
  ).subscribe(user => {
    this.currentUser = user;
    console.log('User state updated in SearchComponent:', this.currentUser);
  });
}
ngOnDestroy(): void {
    this.destroy$.next(true);
    this.destroy$.complete();
  }
}