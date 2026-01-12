export interface Person {
  personId: number;
  name: string;
  currentRank?: string;
  currentDutyTitle?: string;
  careerStartDate?: string;
  careerEndDate?: string;
}

export interface PersonResponse {
  success: boolean;
  message?: string;
  data?: Person[];
  people?: Person[];
  person?: Person;
  id?: number;
}

