import { Component, OnInit } from '@angular/core';
import { RotaService } from '../rota.service';
import { CdkDragDrop } from '@angular/cdk/drag-drop';
import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import jwtDecode from 'jwt-decode';
import apiUrl from '../api';

interface Shift {
  staffId: string;
  date: Date;
  start: Date;
  end: Date;
  isLeave: boolean;
  isOff: boolean;
}

@Component({
  selector: 'app-rota',
  templateUrl: './rota.component.html',
  styleUrls: ['./rota.component.scss']
})
export class RotaComponent implements OnInit {

  stores: any[] = [];
  shifts: Shift[] = [];
  selectedStore: any = null;
  employees: any[] = [];
  staffList: any[] = [];
  selectedEmployee: any = null;
  currentDate: Date = new Date();
  currentYear: number = new Date().getFullYear();
  currentMonth: number = new Date().getMonth();
  daysInMonth: number[] = [];
  employeeSidebarVisible = false;
  isEditMode: boolean = false;
  currentlyEditing: { staffId?: string, day?: number } = {};
  startShift: string = "";
  endShift: string = "";
  tempStartShift: string = '';
  tempEndShift: string = '';
  weeksOfMonth: Array<Array<number>> = [];
  isAdmin$ = new BehaviorSubject<boolean>(this.isAdmin());
  shiftRotaIds: { [key: string]: number } = {};

  setAnnualLeave(staff: any, day: number): void {
    const date = new Date(`${this.currentYear}-${this.currentMonth + 1}-${day}`);
    const start = new Date(date.setHours(0, 0, 0, 0)); // Start of day
    const end = new Date(date.setHours(23, 59, 59, 999)); // End of day

    const annualLeaveShift: Shift = {
      staffId: staff.id,
      date: date,
      start: start,
      end: end,
      isLeave: true,
      isOff: false
    };

    const existingShiftIndex = this.shifts.findIndex(shift =>
      shift.staffId === staff.id &&
      shift.date.getDate() === day &&
      shift.date.getMonth() === this.currentMonth &&
      shift.date.getFullYear() === this.currentYear
    );

    if (existingShiftIndex > -1) {
      this.shifts[existingShiftIndex] = annualLeaveShift;
    } else {
      this.shifts.push(annualLeaveShift);
    }

    // Clear the editing state.
    this.currentlyEditing = {};

    // POST request to save the annual leave shift
    this.http.post(`${apiUrl}/Rota`, {
      rotaId: 0,
      userId: staff.id,
      storeId: this.selectedStore.id,
      date: date.toISOString().split('T')[0], // Format date as 'YYYY-MM-DD'
      startTime: '00:00:00', // Start time as '00:00:00'
      endTime: '00:00:00', // End time as '23:59:59'
      isLeave: true,
      isOff: false
    }).subscribe(response => {
      console.log('Annual leave shift saved:', response);
      this.fetchShiftsForSelectedStore();
    }, error => {
      console.error('Error saving annual leave shift:', error);
    });
  }

// Helper function to format date string
formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = this.padZero(date.getMonth() + 1); // Add 1 to month as it's zero-based
    const day = this.padZero(date.getDate());
    return `${year}-${month}-${day}`;
}

// Helper function to pad single digits with zero
padZero(num: number): string {
    return num < 10 ? `0${num}` : num.toString();
}

getTotalHoursWorkedForWeek(staff: any, week: any): number {
  let totalHours = 0;
  for (const day of week) {
    const shift = this.getShiftForDay(staff, day);
    if (shift && !shift.isLeave) { // Exclude holiday shifts
      const start = new Date(shift.start);
      const end = new Date(shift.end);
      const hoursWorked = (end.getTime() - start.getTime()) / (1000 * 60 * 60);
      totalHours += hoursWorked;
    }
  }
  return totalHours;
}

  

  getWeeksInMonth(month: number, year: number): Array<Array<number>> {
    const weeks = [];
    const firstDate = new Date(year, month, 1);
    const lastDate = new Date(year, month + 1, 0);
    const numDays = lastDate.getDate();

    // The day of the week for the first of the month (0 for Sunday through 6 for Saturday)
    const dayOfWeek = firstDate.getDay();

    // Adjusting the start day; if the month starts on Sunday (0), we want 6 days back to Monday.
    // Otherwise, we go back one less day.
    let start = 1 - (dayOfWeek === 0 ? 6 : dayOfWeek - 1);
    let end = 7 - (dayOfWeek === 0 ? 6 : dayOfWeek - 1);

    while (start <= numDays) {
        weeks.push(Array.from({ length: (end - start + 1) }, (_, idx) => idx + start).filter(day => day >= 1));
        start = end + 1;
        end = end + 7;
        if (end > numDays) end = numDays;
    }

    return weeks;
}


confirmShift(staff: any, day: number, startShift: string, endShift: string): void {
  // Pad the start and end hours with leading zeros if needed
  const paddedStartShift = startShift.padStart(5, '0');
  const paddedEndShift = endShift.padStart(5, '0');

  // Pad the month and day with leading zeros if needed
  const monthString = (this.currentMonth + 1).toString().padStart(2, '0');
  const dayString = day.toString().padStart(2, '0');

  const shiftToAdd = { 
      userId: staff.id,
      storeId: this.selectedStore.id,
      date: `${this.currentYear}-${monthString}-${dayString}`, // Ensure two digits for month and day
      startTime: `${paddedStartShift}:00`, // Ensure two digits for hour
      endTime: `${paddedEndShift}:00`, // Ensure two digits for hour
      isLeave: false, 
      isOff: false 
  };

  this.rotaService.addShift(shiftToAdd).subscribe(response => {
      console.log('Shift added successfully:', response);
      // Handle success as needed
      this.fetchShiftsForSelectedStore();
  }, error => {
      console.error('Error adding shift:', error);
      // Handle error as needed
  });

  // Clear the tempStartShift and tempEndShift
  this.tempStartShift = '';
  this.tempEndShift = '';

  // Also, clear the currentlyEditing to hide the textboxes
  this.currentlyEditing = {};
}


  toggleEmployeeSidebar() {
    this.employeeSidebarVisible = !this.employeeSidebarVisible;
  }


  isAdmin(): boolean {
    const token = localStorage.getItem('access_token');
    if (!token) return false; // If there's no token, user is not an admin
    
    const decodedToken: any = jwtDecode(token);
    
    // Check if the token's role attribute is 'Admin'
    return decodedToken.role === 'Admin';
  }

  checkAdminStatus(): void {
    this.isAdmin$.next(this.isAdmin());
  }

  toggleEditMode() {
    if (this.isAdmin()) {
        this.isEditMode = !this.isEditMode;
        console.log("Toggled edit mode:", this.isEditMode);
    }
  }

  addShift(staff: any, day: number): void {
    // Check if currentlyEditing is already set for this staffId
    if (this.currentlyEditing.staffId === staff.id && this.currentlyEditing.day === day) {
        // If it's already set, clear it to toggle off
        this.currentlyEditing = {};
    } else {
        // If it's not set, set it for this staffId and day
        this.currentlyEditing.staffId = staff.id;
        this.currentlyEditing.day = day;
    }
}

  removeShift(shift: Shift) {
    // Create a unique key for the shift
    const key = shift.staffId + '-' + shift.date.toISOString();
    const rotaId = this.shiftRotaIds[key];
    if (!rotaId) {
      console.error('rotaId not found for shift:', shift);
      return;
    }
  
    // Call the deleteShift method from the RotaService
    this.rotaService.deleteShift(rotaId).subscribe(
      () => {
        console.log('Shift deleted successfully');
        // Remove the shift from the shifts array in your component
        const index = this.shifts.indexOf(shift);
        if (index !== -1) {
          this.shifts.splice(index, 1);
        }
        // Also remove the rotaId from the map
        delete this.shiftRotaIds[key];
      },
      error => {
        console.error('Error deleting shift:', error);
        // Handle error as needed
      }
    );
  }



  getShiftForDay(staff: any, day: number): Shift | undefined {
  const dateToCheck = new Date(this.currentYear, this.currentMonth, day);
  return this.shifts.find(shift =>
    shift.staffId === staff.id &&
    shift.date.getDate() === dateToCheck.getDate() &&
    shift.date.getMonth() === dateToCheck.getMonth() &&
    shift.date.getFullYear() === dateToCheck.getFullYear()
  );
}



  // Calculate total hours worked for a given staff member
  getTotalHoursWorked(staff: any): number {
    const shiftsForStaff = this.shifts.filter(shift => shift.staffId === staff.id);

    let totalHours = 0;

    for (let shift of shiftsForStaff) {
        const startTime = shift.start.getTime();
        const endTime = shift.end.getTime();

        const hoursWorked = (endTime - startTime) / (1000 * 60 * 60);
        totalHours += hoursWorked;
    }

    return totalHours;
}

  assignShift(event: CdkDragDrop<string[]>, staff: any, day: number) {
    console.log('Dropped data:', event.item.data);
    console.log('Assigned to staff:', staff);
    console.log('On day:', day);
  }

  changeMonth(direction: number): void {
    this.currentMonth += direction;
    
    if (this.currentMonth < 0) {
        this.currentMonth = 11; // December
        this.currentYear -= 1;
    } else if (this.currentMonth > 11) {
        this.currentMonth = 0; // January
        this.currentYear += 1;
    }

    this.currentDate = new Date(this.currentYear, this.currentMonth, 1);

    this.updateDaysInMonth();
  }

  constructor(private rotaService: RotaService, private http: HttpClient) { }

  ngOnInit() {
    this.isAdmin();
    this.updateDaysInMonth();
    const currentDate = new Date();
    this.currentYear = currentDate.getFullYear();
    this.currentMonth = currentDate.getMonth(); 
    this.rotaService.getStores().subscribe(stores => {
      console.log("Stores fetched from API:", stores);
      this.stores = stores;
    });
  }

  getDayLabel(day: number): string {
    const date = new Date(this.currentYear, this.currentMonth, day);
    const days = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']; // Starts from Monday
    return days[(date.getDay() - 1 + 7) % 7]; // Adjusting because getDay() returns 0 for Sunday
}
  
  updateDaysInMonth(): void {
    const days = new Date(this.currentYear, this.currentMonth + 1, 0).getDate();
    this.daysInMonth = Array.from({ length: days }, (_, i) => i + 1);
    this.weeksOfMonth = this.getWeeksInMonth(this.currentMonth, this.currentYear);
  }

  selectEmployee(staff: any) {
    this.selectedEmployee = staff;
  }

  onStoreSelect(store: any) {
    this.selectedStore = store;
  
    this.rotaService.getStaffByStore(store.id).subscribe(staff => {
      this.staffList = staff;
    });
  

    this.rotaService.getRotaByStore(store.id).subscribe(rotas => {
      this.shifts = rotas.map((rota: any) => {
        const shift: Shift = {
          staffId: rota.userId,
          date: new Date(rota.date),
          start: new Date(`2000-01-01T${rota.startTime}`),
          end: new Date(`2000-01-01T${rota.endTime}`),
          isLeave: rota.isLeave,
          isOff: rota.isOff
        };
        // Store the rotaId with a unique key
        this.shiftRotaIds[shift.staffId + '-' + shift.date.toISOString()] = rota.rotaId;
        return shift;
      });
      console.log("Rotas fetched from API:", rotas);
    });
  }

  formatTime(time: string): string {
    return time.substring(0, 5);
  }

  fetchShiftsForSelectedStore(): void {
    if (!this.selectedStore) return; // Ensure there is a selected store
    
    this.rotaService.getRotaByStore(this.selectedStore.id).subscribe(rotas => {
      this.shifts = rotas.map((rota: any) => {
        const shift: Shift = {
          staffId: rota.userId,
          date: new Date(rota.date),
          start: new Date(`2000-01-01T${rota.startTime}`),
          end: new Date(`2000-01-01T${rota.endTime}`),
          isLeave: rota.isLeave,
          isOff: rota.isOff
        };
        // Store the rotaId with a unique key for potential future operations
        this.shiftRotaIds[shift.staffId + '-' + shift.date.toISOString()] = rota.rotaId;
        return shift;
      });
      console.log("Rotas fetched from API:", rotas);
    }, error => {
      console.error('Error fetching shifts:', error);
    });
  }
}
