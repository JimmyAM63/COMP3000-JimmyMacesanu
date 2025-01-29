import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import jwtDecode from 'jwt-decode';
import apiUrl from '../api';

interface DecodedToken {
  nameid: string; 
  // Include other properties from your token here.
}

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {
  settingsForm: FormGroup; 
  staffId!: string; 

  constructor(private fb: FormBuilder, private http: HttpClient) {
    this.settingsForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      phoneNumber: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]], // renamed from 'personalEmail'
      address: ['', Validators.required], // renamed from 'homeAddress'
      // Add other fields as necessary.
    });

    const token = localStorage.getItem('access_token'); 
    if (token) {
      const decodedToken = jwtDecode<DecodedToken>(token);
      this.staffId = decodedToken.nameid;

      this.fetchAndPopulateFormData(this.staffId);
    } else {
      console.error('No token found. User might not be authenticated.');
    }
  }

  ngOnInit(): void { }

  fetchAndPopulateFormData(staffId: string): void {
    const apiUrlToUse = `${apiUrl}/Staff/${staffId}`;

    this.http.get(apiUrlToUse).subscribe(
      (data: any) => {
        //console.log('API Response:', data); // Log the raw response. Ensure the keys for 'email' and 'address' exist.

        // Explicitly update form fields. This method avoids any issues related to direct object mapping.
        this.settingsForm.patchValue({
          firstName: data.firstName || '', // Assuming 'firstName' is the correct key from your response.
          lastName: data.lastName || '',
          phoneNumber: data.phoneNumber || '',
          email: data.email || '', // Make sure 'email' matches the key from your API response.
          address: data.address || '', // If 'address' is nullable, it's okay, it defaults to an empty string.
        });

        //console.log('Form Values:', this.settingsForm.value); // Debug: check if form values are set correctly.
      },
      error => {
        console.error('Error fetching staff details:', error);
      }
    );
  }

  onSubmit(): void {
    if (this.settingsForm.valid) {
      const formData = this.settingsForm.value;
      //console.log('Submitting:', formData);

      // Update staff details on the server
      this.updateStaffDetails(this.staffId, formData);
    } else {
      console.error('Form is not valid. Please check the input fields.');
    }
  }

  updateStaffDetails(staffId: string, staffDetails: any): void {
    const apiUrlToUse = `${apiUrl}/Staff/${staffId}`;
  
    // Retrieve the token from local storage or your state management layer.
    const token = localStorage.getItem('access_token');
    
    if (!token) {
      console.error('No authentication token found. Please check your authentication flow.');
      return;
    }
  
    const headers = {
      'Authorization': `Bearer ${token}`
    };
    this.http.put(apiUrlToUse, staffDetails, { headers }).subscribe(
      response => {
        //console.log('Update successful:', response);
        // You might want to provide feedback to the user.
      },
      error => {
        console.error('Error updating staff details:', error);
        // Handle errors and potentially alert the user.
      }
    );
  }

}
