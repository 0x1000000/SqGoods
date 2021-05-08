import { ErrorHandler, Injectable } from '@angular/core';
import { PopupService } from './services/popup-service';

@Injectable()
export class AppErrorHandler implements ErrorHandler {

  constructor(readonly popupService: PopupService) {

  }

  handleError(error: any): void {
    console.error(error);
    this.popupService.showToastMessage('Unexpected error happened!', {type: 'error'});
  }
}
