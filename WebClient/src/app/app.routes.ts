import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { SearchComponent } from './components/search/search.component';
import { AuthGuard } from './guards/auth.guard';
import { FavoritesComponent } from './components/favorites/favorites.component';

export const routes: Routes = [
    {path: 'login', component: LoginComponent},
    {path: 'search', component: SearchComponent,canActivate: [AuthGuard]},
    { path: 'favorites', component: FavoritesComponent, canActivate: [AuthGuard] },
    {path: '', redirectTo: 'login', pathMatch: 'full'},
];
