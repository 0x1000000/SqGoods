import { ChangeDetectorRef, Component, OnDestroy } from '@angular/core';
import { BindToShared, initializeStateTracking, releaseStateTracking } from 'ng-set-state';
import { MessageBox, PopupService, PopupServiceMessageBox } from 'src/app/services/popup-service';

@Component({
  selector: 'app-app-message-box',
  templateUrl: './app-message-box.component.html'
})
export class AppMessageBoxComponent implements OnDestroy, PopupServiceMessageBox {

  @BindToShared()
  currentMessageBox: {data: MessageBox, resolver: (result: string|null) => void}|null;

  constructor(cd: ChangeDetectorRef, popupService: PopupService) {
    const stateHandler = initializeStateTracking<AppMessageBoxComponent>(this, {
      sharedStateTracker: [popupService],
      onStateApplied: () => cd.detectChanges()
    });
    stateHandler.subscribeSharedStateChange();
  }

  ngOnDestroy(): void {
    releaseStateTracking(this);
  }

  onDialogButtonClick(buttonId: string): void{
    if (this.currentMessageBox != null){
      this.currentMessageBox.resolver(buttonId);
    }
  }
}
