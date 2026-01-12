import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../services/api.service';
import { Person, PersonResponse } from '../models/person.model';
import { AstronautDuty, AstronautDutyResponse } from '../models/astronaut-duty.model';

interface AstronautWithDuties {
  person: Person;
  duties: AstronautDuty[];
}

@Component({
  selector: 'app-astronauts',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './astronauts.component.html',
  styleUrl: './astronauts.component.css'
})
export class AstronautsComponent implements OnInit {
  astronauts: AstronautWithDuties[] = [];
  nonAstronauts: Person[] = [];
  loading: boolean = false;
  error: string = '';
  
  // Add astronaut modal
  showAddAstronautModal: boolean = false;
  selectedPersonId: number | null = null;
  
  // Add duty modal
  showAddDutyModal: boolean = false;
  selectedAstronaut: AstronautWithDuties | null = null;
  dutyRank: string = '';
  dutyTitle: string = '';
  dutyStartDate: string = '';
  isRetired: boolean = false;

  constructor(private apiService: ApiService) { }

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading = true;
    this.error = '';
    
    // Load all people
    this.apiService.getPeople().subscribe({
      next: (response) => {
        const allPeople = response.data || response.people || [];
        
        // Separate astronauts from non-astronauts
        this.nonAstronauts = allPeople.filter((p: Person) => !p.currentDutyTitle);
        
        // Load duties for each astronaut
        const astronauts = allPeople.filter((p: Person) => p.currentDutyTitle);
        if (astronauts.length === 0) {
          this.astronauts = [];
          this.loading = false;
          return;
        }
        
        let loadedCount = 0;
        this.astronauts = [];
        
        astronauts.forEach((person: Person) => {
          this.apiService.getAstronautDutiesByName(person.name).subscribe({
            next: (dutyResponse) => {
              this.astronauts.push({
                person,
                duties: dutyResponse.data || []
              });
              loadedCount++;
              if (loadedCount === astronauts.length) {
                this.loading = false;
              }
            },
            error: (err) => {
              console.error('Error loading duties:', err);
              this.astronauts.push({
                person,
                duties: []
              });
              loadedCount++;
              if (loadedCount === astronauts.length) {
                this.loading = false;
              }
            }
          });
        });
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Failed to load data';
        console.error('Error loading people:', err);
      }
    });
  }

  openAddAstronautModal(): void {
    this.showAddAstronautModal = true;
    this.selectedPersonId = null;
  }

  closeAddAstronautModal(): void {
    this.showAddAstronautModal = false;
    this.selectedPersonId = null;
  }

  addAstronaut(): void {
    if (!this.selectedPersonId) {
      this.error = 'Please select a person';
      return;
    }

    const selectedPerson = this.nonAstronauts.find(p => p.personId === this.selectedPersonId!);
    if (!selectedPerson) {
      this.error = 'Selected person not found';
      return;
    }

    this.loading = true;
    this.error = '';
    const today = new Date().toISOString().split('T')[0];
    
    this.apiService.addAstronautDuty(
      selectedPerson.name,
      this.selectedPersonId!,
      'CPT',
      'Spaceman',
      today
    ).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          this.closeAddAstronautModal();
          this.loadData();
        } else {
          this.error = response.message || 'Failed to add astronaut';
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Failed to add astronaut';
        console.error('Error adding astronaut:', err);
      }
    });
  }

  openAddDutyModal(astronaut: AstronautWithDuties): void {
    this.selectedAstronaut = astronaut;
    this.showAddDutyModal = true;
    this.dutyRank = '';
    this.dutyTitle = '';
    this.dutyStartDate = '';
    this.isRetired = false;
  }

  closeAddDutyModal(): void {
    this.showAddDutyModal = false;
    this.selectedAstronaut = null;
    this.dutyRank = '';
    this.dutyTitle = '';
    this.dutyStartDate = '';
    this.isRetired = false;
  }

  onRetiredChange(): void {
    if (this.isRetired) {
      this.dutyTitle = 'RETIRED';
      this.dutyRank = this.selectedAstronaut?.person.currentRank || 'CAPT';
    } else {
      this.dutyTitle = '';
    }
  }

  addDuty(): void {
    if (!this.selectedAstronaut) {
      return;
    }

    if (!this.dutyRank || !this.dutyTitle || !this.dutyStartDate) {
      this.error = 'All fields are required';
      return;
    }

    this.loading = true;
    this.error = '';
    
    this.apiService.addAstronautDuty(
      this.selectedAstronaut.person.name,
      this.selectedAstronaut.person.personId,
      this.dutyRank,
      this.dutyTitle,
      this.dutyStartDate
    ).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          this.closeAddDutyModal();
          this.loadData();
        } else {
          this.error = response.message || 'Failed to add duty';
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Failed to add duty';
        console.error('Error adding duty:', err);
      }
    });
  }
}
