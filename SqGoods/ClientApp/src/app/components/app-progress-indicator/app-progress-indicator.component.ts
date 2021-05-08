import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { BindToShared, ComponentState, ComponentStateDiff, initializeStateTracking, releaseStateTracking, With } from 'ng-set-state';
import { ApiService } from 'src/app/services/api-service.service';

type State = ComponentState<AppProgressIndicatorComponent>;
type NewState = ComponentStateDiff<AppProgressIndicatorComponent>;


@Component({
  selector: 'sq-app-progress-indicator',
  templateUrl: './app-progress-indicator.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppProgressIndicatorComponent implements OnDestroy {

  constructor(api: ApiService, cd: ChangeDetectorRef) {
    initializeStateTracking(this, {sharedStateTracker: api, onStateApplied: () => cd.detectChanges()})
      .subscribeSharedStateChange();
  }

  @BindToShared()
  requestInProgressCounter = 0;

  visible = false;

  @With('requestInProgressCounter').Debounce(500)
  static onCounterChangeShow(state: State): NewState{
    if (state.requestInProgressCounter > 0 && !state.visible){
      return {visible: true};
    }
    return null;
  }

  @With('requestInProgressCounter').Debounce(500)
  static onCounterChangeHide(state: State): NewState{
    if (state.requestInProgressCounter === 0 && state.visible){
      return {visible: false};
    }
    return null;
  }

  ngOnDestroy(): void {
    releaseStateTracking(this);
  }

}
