import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../services/api.service';
import { Person, PersonResponse } from '../models/person.model';

@Component({
  selector: 'app-people',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './people.component.html',
  styleUrl: './people.component.css'
})
export class PeopleComponent implements OnInit {
  people: Person[] = [];
  newPersonName: string = '';
  loading: boolean = false;
  error: string = '';

  constructor(private apiService: ApiService) { }

  ngOnInit(): void {
    this.loadPeople();
  }

  loadPeople(): void {
    this.loading = true;
    this.error = '';
    this.apiService.getPeople().subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.people = response.data;
        } else if (response.people) {
          // Handle different response structure
          this.people = response.people;
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Failed to load people';
        console.error('Error loading people:', err);
      }
    });
  }

  addPerson(): void {
    if (!this.newPersonName.trim()) {
      this.error = 'Name is required';
      return;
    }

    this.loading = true;
    this.error = '';
    this.apiService.createPerson(this.newPersonName.trim()).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          this.newPersonName = '';
          this.loadPeople();
        } else {
          this.error = response.message || 'Failed to create person';
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Failed to create person';
        console.error('Error creating person:', err);
      }
    });
  }

  deletePerson(person: Person): void {
    if (!confirm(`Are you sure you want to delete ${person.name}? This will also delete all astronaut details and duties if they exist.`)) {
      return;
    }

    this.loading = true;
    this.error = '';
    this.apiService.deletePerson(person.personId).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          this.loadPeople();
        } else {
          this.error = response.message || 'Failed to delete person';
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Failed to delete person';
        console.error('Error deleting person:', err);
      }
    });
  }

  isAstronaut(person: Person): boolean {
    return !!person.currentDutyTitle;
  }
}
