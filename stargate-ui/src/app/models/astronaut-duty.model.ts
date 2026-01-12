export interface AstronautDuty {
  id: number;
  personId: number;
  rank: string;
  dutyTitle: string;
  dutyStartDate: string;
  dutyEndDate?: string;
}

export interface AstronautDutyResponse {
  success: boolean;
  message?: string;
  data?: AstronautDuty[];
  astronautDuty?: AstronautDuty;
  id?: number;
}

