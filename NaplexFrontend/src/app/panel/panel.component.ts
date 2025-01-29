import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../auth.service'; 
import { StoreService } from '../store.service';
import { Employee } from '../employee.model';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import jwtDecode from 'jwt-decode';
import apiUrl from '../api';

type Role = 'Sales Advisor' | 'Store Manager' | 'Admin';




@Component({
  selector: 'app-panel',
  templateUrl: './panel.component.html',
  styleUrls: ['./panel.component.scss'],
  animations: [
    trigger('slideDownAnimation', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(-20px)' }),
        animate('0.5s ease-out', style({ opacity: 1, transform: 'translateY(0)' }))
      ]),
      transition(':leave', [
        animate('0.5s ease-out', style({ opacity: 0, transform: 'translateY(-20px)' }))
      ])
    ])
  ]
})
export class PanelComponent implements OnInit {
  registrationForm: FormGroup;
  showAddEmployeeForm = false;
  showRemoveEmployeeForm = false;
  showLendStaffForm = false;
  selectedStore?: number;
  notificationMessage = '';
  showNotification = false;
  employees: Employee[] = [];
  selectedLendStoreFrom?: number;
  selectedLendStoreTo?: number;
  selectedEmployee!: Employee;
  selectedPromotionRole!: Role;
  selectedPromotionRoles: { [key: string]: string } = {};
  selectedRoles: { [id: string]: string } = {};
  selectedPromotionStore?: number;
  showPromoteEmployeeForm = false;
  
  roles = ['Sales Advisor', 'Store Manager', 'Admin'];

  roleIds: { [key: string]: string } = {};

  stores = [
    { id: 1, name: 'Plymouth' },
    { id: 2, name: 'Exeter' },
    { id: 3, name: 'Newton Abbot' },
    { id: 4, name: 'Exmouth' }
  ];
  

  constructor(
    private fb: FormBuilder, 
    private authService: AuthService, 
    private http: HttpClient,
    private storeService: StoreService
  ) {
    this.registrationForm = this.fb.group({
      userName: ['', Validators.required],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', Validators.required],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required],
      role: ['', Validators.required],
      storeId: [null, Validators.required]
    });
  }

  ngOnInit(): void {
    //selectedPromotionRole: string = this.roles[0]; // sets the default role to 'Sales Advisor'
    this.fetchRoles();
  }

  fetchRoles(): void {
    this.http.get<any[]>(`${apiUrl}/Staff/getRoles`).subscribe(
      (roles) => {
        // Transform the array of roles into an object where the role name is the key and the role ID is the value
        this.roleIds = roles.reduce((acc, role) => {
          acc[role.name] = role.id;
          return acc;
        }, {} as { [key: string]: string });
      },
      (error) => {
        console.error('There was an error fetching the roles', error);
      }
    );
  }

  toggleLendStaffForm(){
    this.showLendStaffForm = !this.showLendStaffForm;
  }

  toggleAddEmployeeForm() {
    this.showAddEmployeeForm = !this.showAddEmployeeForm;
  }

  toggleRemoveEmployeeForm(){
    this.showRemoveEmployeeForm = !this.showRemoveEmployeeForm;
  }

  registerEmployee() {
    if (this.registrationForm.valid) {
      const employeeData = this.registrationForm.value;
      this.authService.registerEmployee(employeeData).subscribe(
        res => {
          this.showNotificationMessage(`Employee added successfully.`);
        },
        err => {
          this.showNotificationMessage(`The employee couldn't be added.`);
        }
      );
    } else {
      this.showNotificationMessage(`All the fields are mandatory.`);
    }
  }

  promoteEmployee(employee: Employee) {
    const roleForPromotionName = this.selectedPromotionRoles[employee.id];
    const roleForPromotionId = this.roleIds[roleForPromotionName as Role];
  
    if (!roleForPromotionName || !roleForPromotionId) {
      this.showNotificationMessage(`Invalid or missing role selection.`);
      return;
    }
  
    this.authService.promoteEmployee(employee.id, roleForPromotionId).subscribe(
      () => { // Assuming the response doesn't include the updated employee details
        this.showNotificationMessage(`${employee.firstName} promoted to ${roleForPromotionName} successfully.`);
  
        // Find the employee in the array and update their role
        const index = this.employees.findIndex(emp => emp.id === employee.id);
        if (index !== -1) {
          // Assuming you have a way to get the full role object or name from roleForPromotionId or roleForPromotionName
          // Here we simply update the role's name assuming it's a direct property of Employee
          this.employees[index].role = roleForPromotionName;
  
          // To trigger Angular change detection, create a new array with the updated employee
          this.employees = [...this.employees];
        }
      },
      err => {
        this.showNotificationMessage(err.error.message || `The employee couldn't be promoted.`);
      }
    );
  }

  onPromotionStoreChange() {
    if (this.selectedPromotionStore) {
      this.fetchEmployeesForStore(this.selectedPromotionStore);
    }
  }

  onStoreChange() {
    if (this.selectedStore) {
      this.fetchEmployeesForStore(this.selectedStore);
    }
  }

  onLendStoreChangeFrom() {
    if (this.selectedLendStoreFrom) {
      this.fetchEmployeesForStore(this.selectedLendStoreFrom);
    }
  }
  
  onLendStoreChangeTo() {
    if (this.selectedLendStoreFrom) {
      this.fetchEmployeesForStore(this.selectedLendStoreFrom);
    }
  }

  fetchEmployeesForStore(storeId: number) {
    this.storeService.getEmployeesByStore(storeId).subscribe(
      data => {
        this.employees = data;
      },
      err => {
        this.showNotificationMessage(`There are no employees to be displayed.`);
      }
    );
  }

  lendEmployee(employee: Employee) {
    if (this.selectedLendStoreTo) {
      const userId = employee.id;
      const storeId = this.selectedLendStoreTo;

      this.authService.lendEmployee(userId, storeId.toString()).subscribe(
        res => {
          this.showNotificationMessage(`Employee lent successfully.`);
          if (this.selectedLendStoreFrom) {
            this.fetchEmployeesForStore(this.selectedLendStoreFrom);
          }
        },
        err => {
          this.showNotificationMessage(`The employee couldn't be lent.`);
        }
      );
    } else {
      this.showNotificationMessage(`Please select a destination store.`);
    }
  }

  removeEmployee(employee: Employee) {
    this.authService.removeEmployee(employee.id).subscribe(
      res => {
        // Filter out the removed employee from the employees array
        this.employees = this.employees.filter(emp => emp.id !== employee.id);
  
        // Check if the employees array is empty and display the appropriate message
        if (this.employees.length === 0) {
          this.showNotificationMessage(`No employees to be displayed.`);
        } else {
          this.showNotificationMessage(`Employee removed successfully.`);
        }
  
        // Optionally, if you want to refetch the employees for the store to ensure the UI is up-to-date
        // with the database (in case of external changes or to reset state), you can uncomment the following:
        // if (this.selectedStore) {
        //     this.fetchEmployeesForStore(this.selectedStore);
        // }
      },
      err => {
        this.showNotificationMessage(`The employee couldn't be removed.`);
      }
    );
  }

  showNotificationMessage(message: string): void {
    this.notificationMessage = message;
    this.showNotification = true;
    setTimeout(() => this.showNotification = false, 3000);
  }

  togglePromoteEmployeeForm() {
    this.showPromoteEmployeeForm = !this.showPromoteEmployeeForm;
  }
}
