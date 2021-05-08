import { Injectable } from '@angular/core';
import { IncludeInState, initializeStateTracking } from 'ng-set-state';
import { ApiClient, ApiException, ProblemDetails } from 'src/api-client.generated';
import { PopupService } from './popup-service';

@Injectable({
  providedIn: 'root'
})
export class ApiService extends ApiClient {

  @IncludeInState()
  requestInProgressCounter = 0;

  constructor(readonly popupService: PopupService) {
    super(undefined, {fetch: (url: RequestInfo, init?: RequestInit) => this.fetch(url, init)});

    initializeStateTracking(this);
  }

  handleError(e: any): void{
    if (e instanceof ApiException){
      this.popupService.showToastMessage(e.message, {type: 'error'});
    }
    else if (this.isProblemDetails(e)){
      this.popupService.showToastMessage(e.status + ' - ' + e.title, {type: 'error'});
    }
    else if (this.isMessage(e)){
      this.popupService.showToastMessage(e.message, {type: 'error'});
    }
    else{
      this.popupService.showToastMessage('Unexpected error', {type: 'error'});
    }
    console.error(e);
  }

  async fetch(url: RequestInfo, init?: RequestInit): Promise<Response>{

    try {
      this.requestInProgressCounter++;
      const response = await fetch(url, init);
      return response;
    }
    catch (e: any){
      throw e;
    }
    finally{
      this.requestInProgressCounter--;
    }
  }

  isProblemDetails(e: any): e is ProblemDetails {
    return typeof e.type === 'string' && e.type.startsWith('https://tools.ietf.org/html/rfc7231');
  }

  isMessage(e: any): e is {message: string} {
    return typeof e.message === 'string';
  }
}
