import { IncludeInState, initializeStateTracking, IStateHandler } from 'ng-set-state';
import { ModalDialogButton } from '../lib/modal-dialog/modal-dialog.component';

export abstract class PopupService{
  abstract showToastMessage(text: string, options?: Partial<Omit<ToastMessage, 'text'>>): void;

  abstract showMessageBox(messageBox: MessageBox): Promise<string>;

  abstract showConfirmation(text: string): Promise<boolean>;
}

export class PopupServiceImpl extends PopupService implements PopupServiceMessageBox{
  private readonly _stateHandler: IStateHandler<PopupServiceImpl>;

  @IncludeInState()
  readonly toastMessages: ToastMessage[] = [];

  @IncludeInState()
  readonly toastMessagesVersion: number = 0;

  @IncludeInState()
  readonly currentMessageBox: {data: MessageBox, resolver: (result: string|null) => void}|null;

  constructor() {
    super();
    this._stateHandler = initializeStateTracking<PopupServiceImpl>(this, {});
  }

  showToastMessage(text: string, options?: Partial<Omit<ToastMessage, 'text'>>): void{
    this.toastMessages.push({
      text,
      durationMs: options?.durationMs ?? 5000,
      type: (options?.type) ?? null
    });
    this._stateHandler.modifyStateDiff({toastMessagesVersion: this.toastMessagesVersion + 1});
  }

  showMessageBox(messageBox: MessageBox): Promise<string> {
    if (this.currentMessageBox != null){
      this.currentMessageBox.resolver(null);
    }

    return new Promise<string>(r => {
      const resolver = (id: string) => {
        this._stateHandler.modifyStateDiff({currentMessageBox: null});
        r(id);
      };
      this._stateHandler.modifyStateDiff({currentMessageBox: {data: messageBox, resolver}});
    });
  }

  showConfirmation(text: string): Promise<boolean>{
    return this.showMessageBox({text, buttons: [
      {
        id: 'no',
        title: 'No',
        primary: false},
      {
        id: 'yes',
        title: 'Yes',
        primary: true
      }]})
      .then(id => id === 'yes');
  }
}

export interface PopupServiceToastMessages{
  readonly toastMessages: ToastMessage[];
  readonly toastMessagesVersion: number;
}

export interface PopupServiceMessageBox {
  readonly currentMessageBox: {data: MessageBox, resolver: (result: string|null) => void}|null;
}


export type ToastMessage = {
  readonly text: string;
  readonly durationMs: number,
  readonly type: null | 'error'
};

export type MessageBox = {
  text: string,
  buttons: ModalDialogButton[]
};
