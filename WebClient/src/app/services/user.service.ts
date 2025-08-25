import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable, ReplaySubject } from 'rxjs';
import { environment } from '../../environment/environment';
import { ResponseUser } from '../models/responseuser';

/**
 * UserService is responsible for managing user authentication, user state, and bookmarks.
 * It provides methods for login, user creation, logout, and retrieving the current user.
 */
@Injectable({
  providedIn: 'root'
})
export class UserService {

  private apiUrl = environment.apiUrl; // Use environment variable
  private http = inject(HttpClient);

  private currentUserSource = new ReplaySubject<ResponseUser | null>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor() {this.loadUserFromStorage();}

   /**
   * Logs in the user by sending credentials to the API.
   * Stores the returned user data and token in localStorage and emits the user state.
   * 
   * @param data - The login credentials (username and password)
   * @returns Observable<ResponseUser> - The logged-in user data
   */
  login(data: { UserName: string; Password: string }): Observable<ResponseUser> {
    return this.http.post<ResponseUser>(`${this.apiUrl}/Auth/login`, data)
      .pipe(
        map((response: ResponseUser) => {
          // Store the token in localStorage
          if (response && response.token) {
            localStorage.setItem('access_token', response.token);
            this.currentUserSource.next(response);
          }
          return response;
        })
      );
  }
/**
   * Creates a new user by sending registration data to the API.
   * Stores the returned user data and token in localStorage and emits the user state.
   * 
   * @param data - The registration data (username and password)
   * @returns Observable<ResponseUser> - The created user data
   */
  createUser(data: { UserName: string; Password: string }): Observable<ResponseUser> {
    return this.http.post<ResponseUser>(`${this.apiUrl}/Auth/register`, data).pipe(
      map((response: ResponseUser) => {
        // Store the token in localStorage
        if (response && response.token) {
          localStorage.setItem('access_token', response.token);
          this.currentUserSource.next(response);
        }
        return response;
      })
    );
  }

  getCurrentUser(): Observable<ResponseUser | null> {
    console.log('Getting current user' + this.currentUser$);
    return this.currentUser$; // Expose the current user as an observable
  }
  
 /**
   * Logs out the current user by clearing the token from localStorage and resetting the user state.
   */
  logout(): void {
    localStorage.removeItem('access_token'); // Clear the token
    this.currentUserSource.next(null); // Reset the current user
  }

  /**
 * Loads the current user from localStorage and verifies the user's identity with the backend.
 * If a valid token exists, it fetches the user's details from the backend and updates the current user state.
 * If the token is invalid or missing, it clears the user state.
 */
  loadUserFromStorage(): void {
    const token = localStorage.getItem('access_token');
    if (token) {
      this.http.get<ResponseUser>(`${this.apiUrl}/Auth/me`)
        .subscribe({
          // If the request is successful, emit the user via currentUserSource
          next: (user) => this.currentUserSource.next(user),
          error: () => this.logout()
        });
    } else {
       // If no token exists, emit null to clear the current user state
      this.currentUserSource.next(null);
    }
  }
}
