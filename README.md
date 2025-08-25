# GitHub API Project

## Overview

This project is a modern .NET 8 Web API for searching GitHub repositories, authenticating users, and managing user bookmarks. It follows clean architecture principles, separating concerns across Domain, Application, Infrastructure, and API layers.

## Technologies Used

- **.NET 8 / C# 12**
- **ASP.NET Core Web API**
- **Entity Framework Core** (for data access)
- **JWT Authentication** (with refresh token support)
- **In-Memory Caching** (for session/bookmark storage)
- **Repository & Service Patterns**
- **Swagger/OpenAPI** (for API documentation)

## Design & Best Practices

- **Clean Architecture:**  
  - **Domain Layer:** Contains core business models and logic.
  - **Application Layer:** Handles business use cases and service orchestration.
  - **Infrastructure Layer:** Implements data access, authentication, and external integrations.
  - **API Layer:** Exposes HTTP endpoints and handles request/response formatting.

- **Dependency Injection:** All services and repositories are registered via DI for testability and maintainability.

- **Separation of Concerns:**  
  - Authentication, caching, and business logic are isolated in their respective layers.
  - Models are shared via the Domain project.

- **Extensibility:**  
  - Generic cache service allows storing any object type.
  - Repository pattern enables easy swapping of data sources.

## How to Start the Project

1. **Clone the repository:**
2. **Restore dependencies:**

3. **Update configuration:**
   - Set up your database connection string in `appsettings.json` if using EF Core.
   - Configure JWT settings (issuer, audience, secret) in `appsettings.json`.

4. **Run database migrations (if applicable):**
   
5. **Start the API:**
   
6. **Access Swagger UI:**
   - Navigate to `https://localhost:5001/swagger` (or the port shown in the console).

## Authentication & Refresh Tokens

- **JWT Authentication:**  
  - Users register and log in to receive a JWT access token.
  - Protected endpoints require the `Authorization: Bearer <token>` header.

- **Points for improvement Refresh Token Support:**  
  - When the access token expires, clients can use a refresh token to obtain a new access token without re-authenticating.
  - The refresh token endpoint is typically `/auth/refresh`.
  - Store refresh tokens securely (e.g., HTTP-only cookies or secure storage).
  - The API validates the refresh token and issues a new access token if valid.

## Example Endpoints

- `POST /auth/register` — Register a new user.
- `POST /auth/login` — Authenticate and receive JWT.
- `GET /auth/me` — Get current authenticated user info.
- `GET /github/search?keyword=dotnet` — Search GitHub repositories.
- `POST /github/bookmarks` — Add a repository to bookmarks.
- `GET /github/bookmarks` — List user bookmarks.


# GitHub Repository Gallery

## Project Overview
GitHub Repository Gallery is an Angular-based web application designed to search GitHub repositories, bookmark favorite repositories, and manage user authentication. The application follows best practices for Angular development, including modular design, reusable components, and state management.

---

## Features
- **Search GitHub Repositories**: Search for repositories using keywords and view results with pagination.
- **Bookmark Repositories**: Add repositories to favorites and manage them.
- **User Authentication**: Login and registration functionality with token-based authentication.
- **Favorites Page**: View and manage bookmarked repositories.
- **Responsive Design**: Built with Bootstrap for mobile-friendly layouts.

---

## Best Practices Followed
1. **Modular Design**:
   - Components are organized into feature-specific modules (e.g., `SearchComponent`, `FavoritesComponent`).
   - Services handle business logic and API communication.

2. **State Management**:
   - `ReplaySubject` is used to manage the current user state reactively.
   - Signals are used for reactive state management where applicable.

3. **Reusable Components**:
   - `HeaderComponent` and `FooterComponent` are created for consistent layout across pages.

4. **Error Handling**:
   - API errors are handled gracefully using `catchError` in services.
   - User-friendly error messages are displayed using `ngx-toastr`.

5. **Security**:
   - Token-based authentication is implemented.
   - Sensitive data is cleared on logout.

6. **Responsive Design**:
   - Bootstrap is used for styling and responsive layouts.

---

## Project Structure
src/ ├── app/ │ ├── components/ │ │ ├── search/ │ │ ├── favorites/ │ │ ├── header/ │ │ ├── footer/ │ 
├── services/ │ │ ├── user.service.ts │ │ ├── search.service.ts │ ├── guards/ │ │ ├── auth.guard.ts │ ├── models/ │ │ 
├── githubrepo.ts │ │ ├── responseuser.ts │ │ ├── searchparams.ts │ ├── app.routes.ts │ ├── app.config.ts │ 
├── app.component.ts │ ├── app.module.ts ├── environments/ │ ├── environment.ts │ ├── environment.prod.ts

# GitHub Repository Gallery

Search GitHub repositories, view essential info (name + owner avatar), and **bookmark** your favorites.  
Frontend: Angular. Backend: ASP.NET Core API with JWT auth and in‑memory cache for bookmarks.

---

## Steps to Start the Project (Client)

1. **Clone the Repository**
   ```bash
   git clone https://github.com/your-repo/github-repository-gallery.git
   cd github-repository-gallery
   ```

2. **Install Dependencies**
   ```bash
   npm install
   ```

3. **Set Up Environment Variables**  
   Update `src/environment/environment.ts` and `src/environment/environment.prod.ts` with your API URL(s).

   **Example — `src/environment/environment.ts`:**
   ```ts
   export const environment = {
     production: false,
     apiUrl: 'http://localhost:5241/api' // ASP.NET Core API base URL
   };
   ```

   **Example — `src/environment/environment.prod.ts`:**
   ```ts
   export const environment = {
     production: true,
     apiUrl: 'https://your-prod-api.example.com/api'
   };
   ```

4. **Run the Development Server**
   ```bash
   ng serve
   ```
   The app will be accessible at **http://localhost:4200**.

---

## Technologies Used

- **Angular** — frontend framework
- **Bootstrap** — responsive design
- **RxJS** — reactive programming
- **ngx-toastr** (or **ngx-toast**) — notifications
- **TypeScript** — programming language

---

## Minimal Backend Notes (API)

> Если поднимаешь локально API на .NET 8, проверь CORS и базовый URL, совпадающий с `environment.apiUrl` в Angular.

**Main authentication endpoints:**
- `POST /api/Auth/login` — login, returns JWT:
**Response:** `{ "token": "<JWT>" }`
- `GET /api/Auth/me` — returns the current user profile (by ID tag). Requires header:
```
Authorization: Bearer <JWT>
```

**Bookmarks (example):**
- `POST /api/GitHub/bookmarks?userId={id}` — add repository to bookmarks (in-memory cache)
- `DELETE /api/GitHub/bookmarks/{userId:int}/{name}` — delete bookmark by name (case insensitive)
- `GET /api/GitHub/bookmarks/{userId:int}` — get list of bookmarks

**Notes on /api/Auth/me:**
- Controller is marked with `[Authorize]` — valid JWT is required.
- User ID is taken from `NameIdentifier` or `sub` stamp, it is used to search for record in DB/repository.
- No token is returned in the response — only profile data.

---

## Troubleshooting

- **CORS**: if you get an error like `No 'Access-Control-Allow-Origin'` — enable CORS in ASP.NET Core and specify the origin `http://localhost:4200`.
- **API URL**: 404 in Angular when requesting — check `environment.apiUrl` and the final route in the controller.
---

## Scripts (NPM)

```bash
npm start        # alias for ng serve
npm run build    # prod build
---

## Version
**Current Version**: 1.0.0

---

## How to Start the Project

### Prerequisites
- **Node.js**: Ensure Node.js is installed on your system.
- **Angular CLI**: Install Angular CLI globally using:

Let me know if you need further assistance!---
