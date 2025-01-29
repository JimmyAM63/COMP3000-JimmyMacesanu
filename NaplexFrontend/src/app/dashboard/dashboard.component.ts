import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import Chart from 'chart.js/auto';
import apiUrl from '../api';
import { ChartDataset, TooltipItem, ChartType, Plugin } from 'chart.js';

interface Store {
  id: number;
  storeName: string;
}

interface Sale {
  saleId: number;
  sku: string;
  orderType: string;
  orderNumber: string;
  userId: string;
  storeId: number; // Adjusted to match the property name returned by the API
  saleDate: string;
  saleTime: string;
  revenue: number;
  skU_Type: string;
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

  stores: Store[] = [];
  revenueData: { storeId: number, revenue: number[] }[] = [];

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.fetchStores();
  }

  fetchStores() {
    this.http.get<Store[]>(`${apiUrl}/Stores`).subscribe(
      (response) => {
        this.stores = response;
        this.fetchRevenueData();
      },
      (error) => {
        console.error('Error fetching stores:', error);
      }
    );
  }

  fetchRevenueData() {
    this.http.get<Sale[]>(`${apiUrl}/Sales`).subscribe(
      (salesData) => {
        this.calculateRevenueData(salesData);
        this.renderChart();
      },
      (error) => {
        console.error('Error fetching sales data:', error);
      }
    );
  }

  calculateRevenueData(salesData: Sale[]) {
    //console.log('Sales data:', salesData);
    for (const store of this.stores) {
      const storeSales = salesData.filter(sale => sale.storeId === store.id);
      //console.log('Store ID:', store.id);
      //console.log('Store Sales:', storeSales);
      const storeRevenue = this.calculateTotalRevenue(storeSales);
      //console.log('Store Revenue:', storeRevenue);

      // If storeRevenue is an empty array, populate it with zeros for the last three months
      if (storeRevenue.length === 0) {
        storeRevenue.push(0, 0, 0);
      }

      this.revenueData.push({ storeId: store.id, revenue: storeRevenue });
    }
  }

  calculateTotalRevenue(sales: Sale[]) {
    const currentDate = new Date();
    const currentYear = currentDate.getFullYear();
    const currentMonth = currentDate.getMonth() + 1; // Month is 0-indexed in JavaScript
    const totalRevenue = [];

    for (let i = 2; i >= 0; i--) {
      const month = (currentMonth - i + 12) % 12 || 12; // Ensure positive index, handle year change
      const year = currentMonth - i <= 0 ? currentYear - 1 : currentYear;
      const salesOfMonth = sales.filter(sale => {
        const saleDate = new Date(sale.saleDate);
        return saleDate.getFullYear() === year && saleDate.getMonth() + 1 === month;
      });
      const revenueOfMonth = salesOfMonth.reduce((acc, curr) => acc + curr.revenue, 0);

      totalRevenue.push(revenueOfMonth);
    }

    return totalRevenue;
  }

  renderChart() {
    const arrowIndicatorPlugin: Plugin<'line'> = {
      id: 'arrowIndicator',
      afterDraw: (chart: Chart<'line'>): void => {
        const ctx = chart.ctx;
        chart.data.datasets.forEach((dataset: ChartDataset<'line'>, i: number) => {
          const meta = chart.getDatasetMeta(i);
          if (!meta.hidden && meta.data.length > 1) {
            // Get the last point and the previous point to determine the direction of the line
            const lastPoint = meta.data[meta.data.length - 1].getProps(['x', 'y'], true);
            const previousPoint = meta.data[meta.data.length - 2].getProps(['x', 'y'], true);
    
            // Calculate angle
            const angle = Math.atan2(
              (lastPoint['y'] as number) - (previousPoint['y'] as number),
              (lastPoint['x'] as number) - (previousPoint['x'] as number)
            );
    
            // Setup for drawing the arrow
            ctx.save();
            ctx.translate(lastPoint['x'] as number, lastPoint['y'] as number);
            ctx.rotate(angle);
    
            // Draw arrow that looks like ">"
            ctx.beginPath();
            ctx.moveTo(0, 0); // start at the tip of the arrow
            ctx.lineTo(-7, 3); // line to the bottom of the arrow
            ctx.moveTo(0, 0); // go back to the tip of the arrow
            ctx.lineTo(-7, -3); // line to the top of the arrow
            ctx.strokeStyle = dataset.borderColor as string;
            ctx.stroke();
    
            ctx.restore();
          }
        });
      }
    };
    

    let salesChart: Chart<"line", number[], string> | undefined;
  
    const storeLabels = this.stores.map(store => store.storeName);
  
    const currentDate = new Date();
    const currentMonth = currentDate.getMonth(); // Get the current month (0-indexed)
  
    // Calculate the labels for the last three months
    const lastThreeMonths = [];
    for (let i = 2; i >= 0; i--) {
      const monthIndex = (currentMonth - i + 12) % 12; // Ensure positive index, handle year change
      lastThreeMonths.push(this.getMonthName(monthIndex));
    }
  
    // Prepare data for the chart
    const chartData = [];
    let maxRevenue = 0; // Initialize maximum revenue to 0
  
    // Calculate maxRevenue and prepare chartData simultaneously
    for (const storeData of this.revenueData) {
      const store = this.stores.find(s => s.id === storeData.storeId);
      const storeName = store ? store.storeName : 'Unknown Store';
      const storeRevenue = storeData.revenue;
      const storeMaxRevenue = Math.max(...storeRevenue); // Calculate max revenue for the current store
  
      // Update maxRevenue if the store's max revenue exceeds the current maximum
      if (storeMaxRevenue > maxRevenue) {
        maxRevenue = storeMaxRevenue;
      }

      
  
      chartData.push({
        label: storeName,
        data: storeRevenue,
        backgroundColor: this.getRandomColor(chartData.length), // Use index to ensure consistent colors
        borderColor: this.getRandomColor(chartData.length),
        borderWidth: 1
      });
    }

    // Render the chart
    const ctx = document.getElementById('salesChart') as HTMLCanvasElement;
    salesChart = new Chart(ctx, {
      type: 'line',
      data: {
        labels: lastThreeMonths,
        datasets: chartData
      },
      options: {
        animation: {
          duration: 1500,
          easing: 'easeInOutQuart',
        },
        scales: {
          y: {
            stacked: false,
            suggestedMin: 0,
            suggestedMax: maxRevenue + 10,
            ticks: {
              stepSize: Math.ceil(maxRevenue / 5) || 1
            }
          }
        }
      }
    });
  }

  getMonthName(monthIndex: number) {
    const months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
    return months[monthIndex];
  }

  getRandomColor(index: number) {
    // Predefined array of colors
    const colors = ['#FF5733', '#FFBD33', '#33FF57', '#337BFF', '#FF33E5', '#33FFE0', '#FFAF33', '#7C33FF', '#9B33FF', '#FF333F'];
    return colors[index % colors.length]; // Use index to ensure consistent colors
  }
}
