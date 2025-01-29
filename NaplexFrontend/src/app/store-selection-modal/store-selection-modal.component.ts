import { Component, Output, EventEmitter, OnInit } from '@angular/core';

@Component({
  selector: 'app-store-selection-modal',
  templateUrl: './store-selection-modal.component.html',
  styleUrls: ['./store-selection-modal.component.scss']
})
export class StoreSelectionModalComponent implements OnInit{
  stores = ['Plymouth', 'Exeter', 'Newton Abbot', 'Exmouth']; // You might want to get this from a service instead.
  isVisible: boolean = false;

  constructor() {}

  @Output() storeSelected = new EventEmitter<string>();

  ngOnInit(): void {
    
  }

// Some method that gets called when a store is selected in the modal:
selectStore(storeName: string): void {
  this.storeSelected.emit(storeName);
  // Close the modal or any other logic here
}

  // To show the modal. You can call this method from a parent component or service.
  showModal(): void {
    this.isVisible = true;
  }

  // To hide the modal.
  hideModal(): void {
    this.isVisible = false;
  }
  

  // When the "Cancel" button is pressed.
  cancel(): void {
    this.hideModal();
    console.log('Modal cancelled'); // Optional, just for debugging.
  }

  
}