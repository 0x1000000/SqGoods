import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnDestroy } from '@angular/core';
import { BindToShared, ComponentState, ComponentStateDiff, Emitter, IncludeInState, initializeStateTracking, releaseStateTracking, With, WithAsync } from 'ng-set-state';
import { delayMs } from 'src/app/lib/helpers';
import { PopupService, PopupServiceToastMessages, ToastMessage } from 'src/app/services/popup-service';

type State = ComponentState<AppToastsComponentComponent>;
type NewState = ComponentStateDiff<AppToastsComponentComponent>;

type ToastViewData = {
  readonly el: HTMLElement,
  top: number;
};

type ToastRemainDuration = {
  readonly toast: ToastMessage,
  remainMs: number;

};

@Component({
  selector: 'sq-app-toasts-component',
  templateUrl: './app-toasts-component.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppToastsComponentComponent implements OnDestroy, PopupServiceToastMessages {

  constructor(private cd: ChangeDetectorRef, popupService: PopupService) {
    const stateHandler = initializeStateTracking<AppToastsComponentComponent>(this, {
      sharedStateTracker: [popupService],
      onStateApplied: () => cd.detectChanges()
    });
    stateHandler.subscribeSharedStateChange();
  }

  private _viewData = new Map<ToastMessage, ToastViewData>();

  @BindToShared()
  readonly toastMessages: ToastMessage[];

  @BindToShared()
  readonly toastMessagesVersion: number = 0;

  @IncludeInState()
  @Emitter()
  readonly visibleMessages: ToastMessage[] = [];

  readonly pending: ToastRemainDuration[] = [];

  @With('toastMessagesVersion')
  static onSharedChanged(state: State): NewState {
    if (state.visibleMessages.length === state.toastMessages.length){
      return null;
    }
    const pending: ToastRemainDuration[] = [];
    for (const t of state.toastMessages){
      if (!state.visibleMessages.includes(t)){
        state.visibleMessages.push(t);
        pending.push({toast: t, remainMs: t.durationMs});
      }
    }
    return {
      visibleMessages: state.visibleMessages,
      pending
    };
  }

  @With('visibleMessages')
  static removeShared(state: State): NewState {
    if (state.visibleMessages.length === state.toastMessages.length){
      return null;
    }

    state.toastMessages.length = 0;
    state.toastMessages.push(...state.visibleMessages);

    return {
      toastMessagesVersion: state.toastMessagesVersion + 1
    };
  }

  static split(pending: ToastRemainDuration[]): {now: ToastRemainDuration[], after: ToastRemainDuration[]} {

    let minDuration: number | null = null;
    for (const tr of pending){
      if (minDuration == null || minDuration > tr.remainMs){
        minDuration = tr.remainMs;
      }
    }

    if (minDuration == null){
      return {now: [], after: []};
    }

    const now: ToastRemainDuration[] = [];
    const after: ToastRemainDuration[] = [];

    for (const tr of pending){
      if (tr.remainMs === minDuration){
        now.push(tr);
      }
      else{
        tr.remainMs -= minDuration;
        after.push(tr);
      }
    }
    return {now, after };
  }


  @WithAsync('pending').OnConcurrentLaunchConcurrent()
  static async onPending(getState: () => State): Promise<NewState> {
    const initialState = getState();
    if (initialState.pending.length > 0){

      const {now, after} = AppToastsComponentComponent.split(initialState.pending);

      await delayMs(now[0].remainMs);

      const currentState = getState();

      for (const oldP of now){
        const index = initialState.visibleMessages.indexOf(oldP.toast);
        if (index >= 0){
          initialState.visibleMessages.splice(index, 1);
        }
      }

      return {
        visibleMessages: currentState.visibleMessages,
        pending: after
      };
    }
    return null;
  }

  ngOnDestroy(): void {
    releaseStateTracking(this);
  }

  onToastCreated(el: HTMLElement, t: ToastMessage): void{
    const index = this.toastMessages.indexOf(t);
    if (index < 0){
      throw new Error('Toast error');
    }
    this._viewData.set(t, {el, top: 0});
    this.recalculateTop();
  }

  onToastDestroyed(el: HTMLElement, t: ToastMessage): void{
    this._viewData.delete(t);
    this.recalculateTop();
  }

  getTop(t: ToastMessage): number{
    const data = this._viewData.get(t);
    if (!data){
      return 0;
    }
    return data.top;
  }

  private recalculateTop(): void{
    setTimeout(() => {
      let top = 0;
      for (const t of this.toastMessages) {
        const data = this._viewData.get(t);
        if (!data){
          throw new Error('Toast el error');
        }
        data.top = top;
        top +=  data.el.clientHeight + 20;
      }
      this.cd.detectChanges();
    });
  }
}
