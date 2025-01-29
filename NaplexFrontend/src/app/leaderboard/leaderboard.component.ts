import { Component, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { LeaderboardService, Store } from './leaderboard.service';
import { SalesService } from '../sales.service';
import { TargetService } from '../targets.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

interface Advisor {
  name: string;
  revenue: number;
  new: number;
  upgrade: number;
  unlimited: number;
  hbb: number;
  hbbup: number;
  additional: number;
}

@Component({
  selector: 'app-leaderboard',
  templateUrl: './leaderboard.component.html',
  styleUrls: ['./leaderboard.component.scss']
})
export class LeaderboardComponent implements OnInit {
  selectedDate: string = '';
  leaderboardData: Advisor[] = [];
  stores: Store[] = [];
  private unsubscribe$ = new Subject<void>();

  constructor(
    private datePipe: DatePipe, 
    private salesService: SalesService,
    private targetService: TargetService,
    private leaderboardService: LeaderboardService
  ) {}

  ngOnInit(): void {
    const today = new Date();
    this.selectedDate = this.formatDate(today);
    this.loadLeaderboard();
    this.loadStores();
  }

  loadLeaderboard(): void {
    // Fetch daily sales data for revenue
    this.salesService.getAllSales().subscribe(salesData => {
      const advisorMap = new Map<string, Advisor>();

      // Filter sales data based on the selected date
      const filteredSales = salesData.filter(sale => this.formatDate(new Date(sale.saleDate)) === this.selectedDate);

      // Retrieve staff names and initialize revenue
      Promise.all(filteredSales.map(sale => this.getStaffName(sale.userId))).then(staffNames => {
        filteredSales.forEach((sale, index) => {
          const advisorName = `${staffNames[index].firstName} ${staffNames[index].lastName}` || 'Unknown';
          const saleRevenue = parseFloat(sale.revenue || '0');

          if (advisorMap.has(advisorName)) {
            const advisor = advisorMap.get(advisorName)!;
            advisor.revenue += saleRevenue;
          } else {
            advisorMap.set(advisorName, {
              name: advisorName,
              revenue: saleRevenue,
              new: 0,
              upgrade: 0,
              unlimited: 0,
              hbb: 0,
              hbbup: 0,
              additional: 0
            });
          }
        });

        // Fetch daily targets for each advisor
        Promise.all(Array.from(advisorMap.keys()).map((advisorName, index) => {
          const userId = filteredSales.find(sale => {
            const staffName = `${staffNames[index].firstName} ${staffNames[index].lastName}` || 'Unknown';
            return staffName === advisorName;
          })?.userId;

          return userId ? this.targetService.getTargetsByUserAndMonth(userId, new Date(this.selectedDate)).toPromise() : Promise.resolve([]);
        })).then(targetsDataArray => {
          targetsDataArray.forEach((targetsData, index) => {
            const advisorName = Array.from(advisorMap.keys())[index];
            const advisor = advisorMap.get(advisorName);

            if (targetsData && advisor) {
              // Filter targetsData for the specific date
              const dailyTargets = targetsData.filter(target => {
                if (!target.targetDate) {
                  console.warn(`Target with missing date for advisor ${advisorName}:`, target);
                  return false;
                }
                return this.formatDate(this.convertToDate(target.targetDate)) === this.selectedDate;
              });
              dailyTargets.forEach(target => {
                advisor.new += target.newAct ?? 0;
                advisor.upgrade += target.upgradesAct ?? 0;
                advisor.unlimited += target.unlimitedAct ?? 0;
              });
            }
          });

          // Convert map to array of Advisor objects
          this.leaderboardData = Array.from(advisorMap.values());

          // Sort leaderboardData by revenue in descending order
          this.leaderboardData.sort((a, b) => b.revenue - a.revenue);
        });
      });
    });
  }

  loadStores(): void {
    this.leaderboardService.getStores()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(data => {
        this.stores = data;
      });
  }

  getStaffName(userId: string): Promise<{ firstName: string, lastName: string }> {
    return this.salesService.getStaffName(userId).toPromise().then(response => {
      return {
        firstName: response?.firstName || 'Unknown',
        lastName: response?.lastName || 'Unknown'
      };
    }).catch(error => {
      console.error('Error fetching staff name:', error);
      return {
        firstName: 'Unavailable',
        lastName: 'Unavailable'
      };
    });
  }

  onDateSelect(event: any) {
    const selectedDate = new Date(event);
    if (!isNaN(selectedDate.getTime())) {
      this.selectedDate = this.formatDate(selectedDate);
      this.loadLeaderboard();
    } else {
      console.error('Invalid date selected:', event);
    }
  }

  private formatDate(date: Date): string {
    const formattedDate = this.datePipe.transform(date, 'yyyy-MM-dd') || '';
    //console.log('Formatted date:', formattedDate);
    return formattedDate;
  }

  private convertToDate(dateStr: string): Date {
    //console.log('Converting to date from string:', dateStr);
    if (!dateStr) {
      console.error('Invalid date string:', dateStr);
      return new Date();
    }
    const [year, month, day] = dateStr.split('-').map(Number);
    const date = new Date(year, month - 1, day);
    //console.log('Converted date:', date);
    return date;
  }

  startResizing(event: MouseEvent) {
    const target = event.target as HTMLElement;
    const th = target.closest('th');
    if (!th) return;
    const startX = event.pageX;
    const startWidth = th.offsetWidth;

    const mouseMoveHandler = (e: MouseEvent) => {
      const newWidth = startWidth + (e.pageX - startX);
      th.style.width = newWidth + 'px';
    };

    const mouseUpHandler = () => {
      document.removeEventListener('mousemove', mouseMoveHandler);
      document.removeEventListener('mouseup', mouseUpHandler);
    };

    document.addEventListener('mousemove', mouseMoveHandler);
    document.addEventListener('mouseup', mouseUpHandler);
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
}
