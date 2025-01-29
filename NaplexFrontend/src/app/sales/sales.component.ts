import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import jwt_decode from 'jwt-decode';
import { SalesService } from '../sales.service';
import apiUrl from '../api';
import { AuthService } from '../auth.service';

interface Sale {
  saleId?: number;
  sku: string;
  orderType: string;
  orderNumber: string;
  saleDate: string;
  saleTime: string;
  revenue?: string;
  storeId: number;
  staffName?: string;
  userId: string;
  storeName?: string;
  skU_Type?: string;
  isAdditional: boolean;
  isDiscounted: boolean; // Add this line
}

interface DecodedToken {
  nameid: string;
}

@Component({
  selector: 'app-sales',
  templateUrl: './sales.component.html',
  styleUrls: ['./sales.component.scss']
})
export class SalesComponent implements OnInit {
  showModal: boolean = false;
  currentUserId: string = '';
  isAdmin: boolean = false;
  saleToDelete: number | null = null;
  salesData: Sale = {
    sku: '',
    orderType: '',
    orderNumber: '',
    saleDate: '',
    saleTime: '',
    userId: '',
    storeId: 0,
    isAdditional: false,
    isDiscounted: false // Initialize it here
  };
  salesHistory: Sale[] = [];
  staffName: string = '';
  sidebarExpanded = false;

  // Mapping between front-end display values and backend expected values
  typeMapping: { [key: string]: string } = {
    'Upgrade': 'Retention',
    'New': 'Acquisition'
  };

  toggleSidebar(): void {
    this.sidebarExpanded = !this.sidebarExpanded;
  }

  constructor(private http: HttpClient, private salesService: SalesService, private authService: AuthService) { }

  ngOnInit(): void {
    this.checkAdminStatus();
    this.setCurrentUserId();
    this.loadSalesHistory();
  }

  async loadSalesHistory(): Promise<void> {
    try {
      const token = localStorage.getItem('access_token');
      if (!token) throw new Error('No token found');
  
      const decodedToken: any = jwt_decode(token);
      if (!decodedToken.nameid) throw new Error('Invalid token data');
  
      const userId = decodedToken.nameid;
  
      // Fetch sales history
      const data: Sale[] | undefined = await this.salesService.getAllSales().toPromise();
  
      if (!data) {
        throw new Error('No sales data found');
      }
  
      // Fetch store names for each sale
      for (const sale of data) {
        sale.storeName = await this.getStoreName(sale.storeId);
        sale.staffName = await this.getStaffName(sale.userId);
      }
  
      // Reverse the array to display the latest sale first
      this.salesHistory = data.reverse();
  
      //console.log('Sales history:', this.salesHistory);
    } catch (error) {
      console.error('Error fetching sales history:', error);
    }
  }

  checkAdminStatus(): void {
    this.isAdmin = this.authService.isAdmin();  // Get admin status from AuthService
  }
  
  setCurrentUserId(): void {
    const token = localStorage.getItem('access_token');
    if (token) {
      const decodedToken: any = jwt_decode(token);
      this.currentUserId = decodedToken.nameid;
    }
  }

  confirmDeleteSale(index: number): void {
    this.saleToDelete = index;
    this.showModal = true;
  }

  deleteSale(index: number): void {
    const sale = this.salesHistory[index];
    if (sale && sale.saleId !== undefined) {
      const saleIdToDelete: number = sale.saleId;
      this.salesService.deleteSale(saleIdToDelete).subscribe(
        () => {
          this.salesHistory.splice(index, 1);
        },
        (error) => {
          console.error('Error deleting sale:', error);
        }
      );
    } else {
      console.error('Invalid sale or saleId.');
    }
  }

  handleModalClick(confirmDeletion: boolean): void {
    if (confirmDeletion && this.saleToDelete !== null) {
      this.deleteSale(this.saleToDelete);
    }
    this.showModal = false;
    this.saleToDelete = null;
  }

  async getStaffName(userId: string): Promise<string> {
    try {
      // Fetch the staff data using the ID from the token
      const response = await this.http.get<{ firstName?: string }>(`${apiUrl}/Staff/${userId}`).toPromise();
      return response?.firstName || 'N/A'; // Use optional chaining to handle possible undefined
    } catch (error) {
      console.error('Error fetching staff name:', error);
      return 'Unavailable'; // Fallback if fetching fails
    }
  }

  async getStoreName(storeId: number): Promise<string> {
    try {
      // Fetch the store data using the storeId
      const response = await this.http.get<{ storeName: string }>(`${apiUrl}/Stores/${storeId}`).toPromise();
      return response?.storeName || 'N/A'; // Use optional chaining to handle possible undefined
    } catch (error) {
      console.error('Error fetching store name:', error);
      return 'Unknown'; // Fallback if fetching fails
    }
  }

  onSubmit(): void {
    // Decode the access token to extract the userId
    const token = localStorage.getItem('access_token');
    if (!token) {
      console.error('Access token not found');
      return;
    }

    const decodedToken: any = jwt_decode(token);
    if (!decodedToken || !decodedToken.nameid) {
      console.error('Invalid access token format');
      return;
    }

    // Check if selected orderType is 'New_Additional'
  if (this.salesData.orderType === 'New_Additional') {
    this.salesData.isAdditional = true; // Set IsAdditional to true
    this.salesData.orderType = 'Acquisition'; // Set OrderType to 'Acquisition'
  } else {
    // Use typeMapping for other scenarios and set isAdditional to false
    this.salesData.orderType = this.typeMapping[this.salesData.orderType] || this.salesData.orderType;
    this.salesData.isAdditional = false; // Ensure it's false for other types
  }

    // Assign the decoded nameid to userId
    this.salesData.userId = decodedToken.nameid;

    // Format the time string to HH:MM:00 format
    this.salesData.saleTime = this.formatTime(this.salesData.saleTime);

    // Retrieve the store ID based on the user ID
    this.http.get<any[]>(`${apiUrl}/Stores/user/${this.salesData.userId}/stores`).subscribe(
      (response) => {
        if (Array.isArray(response) && response.length > 0) {
          // Extract the store ID from the response
          this.salesData.storeId = response[0].id;

          // Send the POST request to add the new sale
          this.salesService.addSale(this.salesData).subscribe(
            (newSale: Sale) => {
              // Retrieve the staff name for the newly added sale
              this.getStaffName(newSale.userId).then(staffName => {
                newSale.staffName = staffName;
                // Update the sales history with the new sale
                this.salesHistory.push(newSale);
                this.clearForm();
                this.loadSalesHistory();
              });
            },
            (error) => {
              console.error('Error adding sale:', error);
            }
          );
        } else {
          console.error('No store found for the user.');
        }
      },
      (error) => {
        console.error('Error retrieving store:', error);
      }
    );

    //console.log('Form submitted:', this.salesData);
  }

  clearForm(): void {
    this.salesData = {
      sku: '',
      orderType: '',
      orderNumber: '',
      saleDate: '',
      saleTime: '',
      storeId: 0, // Default to store 1
      userId: '', // Preserve userId
      isAdditional: false,
      isDiscounted: false // Reset the isDiscounted field
    };
  }

  formatTime(time: string): string {
    return time + ':00';
  }

  formatDate(date: string, time: string): string {
    const options: Intl.DateTimeFormatOptions = {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: 'numeric',
      minute: 'numeric'
    };

    const formattedDate = new Date(date + ' ' + time);
    return formattedDate.toLocaleDateString('en-US', options);
  }
}
