import { Routes } from '@angular/router';
import { PeopleComponent } from './people/people.component';
import { AstronautsComponent } from './astronauts/astronauts.component';

export const routes: Routes = [
  { path: '', redirectTo: '/people', pathMatch: 'full' },
  { path: 'people', component: PeopleComponent },
  { path: 'astronauts', component: AstronautsComponent }
];
