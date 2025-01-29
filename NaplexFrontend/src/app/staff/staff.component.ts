import { Component, OnInit } from '@angular/core';
import { StoreService } from '../store.service';
import { Employee } from '../employee.model';

@Component({
  selector: 'app-staff',
  templateUrl: './staff.component.html',
  styleUrls: ['./staff.component.scss']
})
export class StaffComponent implements OnInit {
  stores: string[] = [];
  staffByStore: { [key: string]: Employee[] } = {}; // Object to store staff by store

  constructor(private storeService: StoreService) { }

  ngOnInit() {
    this.storeService.getStores().subscribe(data => {
      this.stores = data.map(store => store.storeName);
      // Fetch staff data for each store
      this.stores.forEach(storeName => {
        // Find the corresponding store ID based on the store name
        const store = data.find(s => s.storeName === storeName);
        if (store) {
          // If store is found, get employees by store ID
          this.storeService.getEmployeesByStore(store.id).subscribe(staff => {
            this.staffByStore[storeName] = staff;
          });
        }
      });
    });
  }

  // Method to get staff by store
  getStaffByStore(storeName: string) {
    return this.staffByStore[storeName] || [];
  }
}
