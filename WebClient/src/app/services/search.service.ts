import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';
import { HttpClient } from '@angular/common/http';
import { GithubRepo } from '../models/githubrepo';
import { SearchParams } from '../models/searchparams';

/**
 * SearchService is responsible for interacting with the GitHub API and backend services
 * to fetch repositories based on search parameters and manage bookmarks.
 */
@Injectable({
  providedIn: 'root'
})
export class SearchService {

  constructor() { }
  private apiUrl = environment.apiUrl; // Use environment variable
  private http = inject(HttpClient);
  
  /**
   * Searches for GitHub repositories based on the provided search parameters.
   * 
   * @param params - The search parameters (keywords, page, perPage)
   * @returns Observable<GithubRepo[]> - The list of repositories matching the search criteria
   */
  searchRepositories(params: SearchParams): Observable<GithubRepo[]> {
    const queryParams = new URLSearchParams();
    // Map `SearchParams` properties to query parameters
    queryParams.append('keyword', params.keywords); // Map `keywords` to `q`
    queryParams.append('page', params.page.toString()); // Convert `page` to string
    queryParams.append('perPage', params.perPage.toString()); // Convert `perPage` to string
    return this.http.get<GithubRepo[]>(`${this.apiUrl}/GitHub/search?${queryParams.toString()}`);
  }

 /**
   * Adds a repository to the user's bookmarks by sending the repository data to the API.
   * 
   * @param repo - The repository to add to bookmarks
   * @param userId - The ID of the current user
   * @returns Observable<{ addedBookmark: GithubRepo; updatedBookmarks: GithubRepo[] }> - The added bookmark and updated list
   */
  addToBookmarks(repo: GithubRepo, userId: number): Observable<GithubRepo[]> {
    console.log('Adding to bookmarks: ', repo, userId);
    return this.http.post<GithubRepo[]>(
      `${this.apiUrl}/GitHub/bookmarks/${userId}`,repo,);
  }

   /**
   * Removes a repository from the user's bookmarks by sending the repository data to the API.
   * 
   * @param repo - The repository to remove from bookmarks
   * @param userId - The ID of the current user
   * @returns Observable<GithubRepo[]> - The updated list of bookmarks
   */
  removeBookmark(repo: GithubRepo, userId: number): Observable<GithubRepo[]> {
    console.log('Removing bookmark:', repo, userId);
    const name = encodeURIComponent(repo.name);
    return this.http.delete<GithubRepo[]>(
      `${this.apiUrl}/GitHub/bookmarks/${userId}/${name}`);
  }

  getBookmarks(userId: number): Observable<GithubRepo[]> {
    return this.http.get<GithubRepo[]>(`${this.apiUrl}/GitHub/bookmarks/${userId}`);
  }
}
