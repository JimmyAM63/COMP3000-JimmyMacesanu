export interface Employee {
  id: string;
  userName?: string;
  firstName?: string;
  lastName?: string;
  email?: string;
  role?: string;
  storeId?: number;
  selectedPromotionRole?: string;
  // Add any other fields that an Employee might have based on your API
}