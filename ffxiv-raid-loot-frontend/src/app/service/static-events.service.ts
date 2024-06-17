// src/app/services/static-events.service.ts
import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class StaticEventsService {
  private recomputePGSEvent = new Subject<void>();

  // Observable stream
  recomputePGS$ = this.recomputePGSEvent.asObservable();

  // Service message commands
  triggerRecomputePGS() {
    this.recomputePGSEvent.next();
  }
}