import { Component, OnInit } from '@angular/core';
import { StoreService } from '../store.service';
import { TargetService } from '../targets.service';
import { Employee } from '../employee.model';
import { BehaviorSubject } from 'rxjs';
import jwtDecode from 'jwt-decode';

interface KPI {
  actual: number;
  percentage: number;
  target: number;
}

export interface EmployeeKPI {
  name: string;
  new: KPI;
  talkMobile: KPI;
  upgrades: KPI;
  hbb: KPI;
  hbbup: KPI;
  rev: KPI;
  unlimited: KPI;
  insurance: KPI;
  additional: KPI;
  hbbvsupg: KPI;
  newvsupg: KPI;
  isEditing: boolean; // Flag for edit mode
  targetId: number; // Target ID for updating targets
  userId: string; // User ID for updating targets
}

interface StoreData {
  id: number;
  name: string;
  employees: EmployeeKPI[];
}

@Component({
  selector: 'app-targets',
  templateUrl: './targets.component.html',
  styleUrls: ['./targets.component.scss']
})
export class TargetsComponent implements OnInit {
  stores: StoreData[] = [];
  chosenMonth: string = ''; 
  minDate: Date = new Date(2000, 0, 1);
  maxDate: Date = new Date();
  isAdmin$ = new BehaviorSubject<boolean>(this.isAdmin());

  constructor(private storeService: StoreService, private targetService: TargetService) {}

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

  ngOnInit() {
    this.isAdmin();
    const currentDate = new Date();
    this.chosenMonth = currentDate.toLocaleString('default', { month: 'long', year: 'numeric' });
    this.fetchTargets();
  }

  fetchTargets() {
    if (!this.chosenMonth) return;
  
    const year = parseInt(this.chosenMonth.split(' ')[1]);
    const month = new Date(Date.parse(this.chosenMonth)).getMonth() + 1;
    const monthYearStr = `${year}-${('0' + month).slice(-2)}`;
  
    this.storeService.getStores().subscribe(stores => {
      this.stores = stores.map(store => ({
        id: store.id,
        name: store.storeName,
        employees: [],
        storeId: store.id // Store the storeId along with other store data
      }));
  
      this.stores.forEach(storeData => {
        this.storeService.getEmployeesByStore(storeData.id).subscribe(employees => {
          storeData.employees = employees.map(employee => this.convertToEmployeeKPI(employee));
          employees.forEach(employee => {
            console.log(`User for employee ${employee.firstName} ${employee.lastName} → ID:`, employee.id);

            if (this.chosenMonth) {
              // Fetch targets by store and month
              this.targetService.getTargetsByStoreAndMonth(storeData.id.toString(), new Date(year, month - 1)).subscribe(storeTargets => {
                // Filter targets by user ID
                const userTargets = storeTargets.filter(target => target.userId === employee.id);
                const target = userTargets[0];
  
                if (target) {
                  const employeeIndex = storeData.employees.findIndex(emp => emp.name === `${employee.firstName} ${employee.lastName}`);
                  storeData.employees[employeeIndex].new = {
                    actual: target.newAct ?? 0,
                    target: target.newTar ?? 0,
                    percentage: (target.newAct ?? 0) / (target.newTar ?? 1) * 100
                  };
                  storeData.employees[employeeIndex].talkMobile = {
                    actual: target.talkMobileAct ?? 0,
                    target: target.talkMobileTar ?? 0,
                    percentage: (target.talkMobileAct ?? 0) / (target.talkMobileTar ?? 1) * 100
                  };
                  storeData.employees[employeeIndex].upgrades = {
                    actual: target.upgradesAct ?? 0,
                    target: target.upgradesTar ?? 0,
                    percentage: (target.upgradesAct ?? 0) / (target.upgradesTar ?? 1) * 100
                  };
                  storeData.employees[employeeIndex].hbb = {
                    actual: target.hbbAct ?? 0,
                    target: target.hbbTar ?? 0,
                    percentage: (target.hbbAct ?? 0) / (target.hbbTar ?? 1) * 100
                  };
                  storeData.employees[employeeIndex].hbbup = {
                    actual: target.hbbUpAct ?? 0,
                    target: target.hbbUpTar ?? 0,
                    percentage: (target.hbbUpAct ?? 0) / (target.hbbUpTar ?? 1) * 100
                  };
                  storeData.employees[employeeIndex].unlimited = {
                    actual: target.unlimitedAct ?? 0,
                    target: target.unlimitedTar ?? 0,
                    percentage: (target.unlimitedAct ?? 0) / (target.unlimitedTar ?? 1) * 100
                  };
                  storeData.employees[employeeIndex].additional = {
                    actual: target.additionalAct ?? 0,
                    target: target.additionalTar ?? 0,
                    percentage: (target.additionalAct ?? 0) / (target.additionalTar ?? 1) * 100
                  };
                  storeData.employees[employeeIndex].targetId = target.targetId ?? 0;
                  storeData.employees[employeeIndex].userId = employee.id;
                }
              });
            }
          });
        });
      });
  
      // Sort stores based on their IDs
      this.stores.sort((a, b) => a.id - b.id);
    });
  }
  
  

  convertToEmployeeKPI(employeeData: Employee): EmployeeKPI {
    return {
      name: `${employeeData.firstName} ${employeeData.lastName}`,
      new: { actual: 0, percentage: 0, target: 0 },
      talkMobile: { actual: 0, percentage: 0, target: 0 },
      upgrades: { actual: 0, percentage: 0, target: 0 },
      hbb: { actual: 0, percentage: 0, target: 0 },
      hbbup: { actual: 0, percentage: 0, target: 0 },
      rev: { actual: 0, percentage: 0, target: 0 },
      unlimited: { actual: 0, percentage: 0, target: 0 },
      insurance: { actual: 0, percentage: 0, target: 0 },
      additional: { actual: 0, percentage: 0, target: 0 },
      hbbvsupg: { actual: 0, percentage: 0, target: 0 },
      newvsupg: { actual: 0, percentage: 0, target: 0 },
      isEditing: false,
      targetId: 0,
      userId: employeeData.id
    };
  }

  previousMonth() {
    const currentDate = new Date(this.chosenMonth + ' 1');
    currentDate.setMonth(currentDate.getMonth() - 1);
    this.chosenMonth = currentDate.toLocaleString('default', { month: 'long', year: 'numeric' });
    this.fetchTargets();
  }
  
  nextMonth() {
    const currentDate = new Date(this.chosenMonth + ' 1');
    currentDate.setMonth(currentDate.getMonth() + 1);
    this.chosenMonth = currentDate.toLocaleString('default', { month: 'long', year: 'numeric' });
    this.fetchTargets();
  }

  filterDate(date: Date | null): boolean {
    return true;
  }

  getTotalActual(kpiKey: keyof EmployeeKPI, store: StoreData): number {
    return store.employees.reduce((sum, employee) => {
      const kpiValue = employee[kpiKey] as KPI;
      if (typeof kpiValue !== 'string' && kpiValue.hasOwnProperty('actual')) {
        return sum + kpiValue.actual;
      }
      return sum;
    }, 0);
  }

  getTotalPercentage(kpiKey: keyof EmployeeKPI, store: StoreData): number {
    if (!store.employees.length) return 0;
    
    let totalActual = 0;
    let totalTarget = 0;
    let validEmployeesCount = 0;
  
    store.employees.forEach(employee => {
      const kpiValue = employee[kpiKey] as KPI;
      if (typeof kpiValue !== 'string' && kpiValue.hasOwnProperty('actual') && kpiValue.hasOwnProperty('target')) {
        totalActual += kpiValue.actual;
        totalTarget += kpiValue.target;
        validEmployeesCount++;
      }
    });
  
    // Ensure at least one valid employee exists
    if (validEmployeesCount === 0) return 0;
  
    // Calculate total percentage based on total actual and total target
    const totalPercentage = validEmployeesCount !== 0 ? ((totalActual / totalTarget) / validEmployeesCount) * 100 : 0;
  
    return totalPercentage;
  }
  

  getTotalTarget(kpiKey: keyof EmployeeKPI, store: StoreData): number {
    if (!store.employees.length) return 0;

    const totalTarget = store.employees.reduce((sum, employee) => {
      const kpiValue = employee[kpiKey] as KPI;
      if (typeof kpiValue !== 'string' && kpiValue.hasOwnProperty('target')) {
        return sum + kpiValue.target;
      }
      return sum;
    }, 0);

    return totalTarget;
  }

  toggleEdit() {
    if (this.isAdmin()) {
      this.stores.forEach(storeData => {
        storeData.employees.forEach(employee => {
          employee.isEditing = !employee.isEditing;
          if (!employee.isEditing) {
            // Save changes when exiting edit mode
            this.saveEmployeeChanges(storeData, employee);
          }
        });
      });
    }
  }

  editEmployee(store: StoreData, employee: EmployeeKPI) {
    employee.isEditing = true;
  }

  saveEmployeeChanges(store: StoreData, employee: EmployeeKPI) {
    employee.isEditing = false;
  
    const {
      targetId,
      userId,
      new: { target: newTar },
      talkMobile: { target: talkMobileTar },
      additional: { target: additionalTar },
      upgrades: { target: upgradesTar },
      hbb: { target: hbbTar },
      hbbup: { target: hbbUpTar },
      rev: { target: revTar },
      unlimited: { target: unlimitedTar },
      insurance: { target: insuranceTar }
    } = employee;
  
    const storeId = store.id;
    if (!this.chosenMonth) return;
  
    const year = parseInt(this.chosenMonth.split(' ')[1]);
    const month = new Date(this.chosenMonth + ' 1').getMonth() + 1;
    const monthYearStr = `${year}-${('0' + month).slice(-2)}-01`; // e.g., "2025-04-01"
  
    // Only send request if at least one value > 0
    if (
      newTar > 0 || talkMobileTar > 0 || additionalTar > 0 || upgradesTar > 0 ||
      hbbTar > 0 || hbbUpTar > 0 || revTar > 0 || unlimitedTar > 0 || insuranceTar > 0
    ) {
      // Unified update call (backend will create if targetId doesn't exist)
      this.targetService.updateTarget(targetId, {
        userId,
        storeId,
        targetDate: monthYearStr,
        newTar,
        talkMobileTar,
        additionalTar,
        upgradesTar,
        hbbTar,
        hbbUpTar,
        revTar,
        unlimitedTar,
        insuranceTar
      }).subscribe({
        next: () => {
          console.log('Target created or updated successfully');
          this.fetchTargets(); // Refresh UI
        },
        error: err => {
          console.error('Failed to create/update target:', err);
        }
      });
    }
  }
}
