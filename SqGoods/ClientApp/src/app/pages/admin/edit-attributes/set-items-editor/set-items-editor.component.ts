import { ChangeDetectionStrategy, ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ComponentState, ComponentStateDiff, Emitter, initializeStateTracking, With, WithAsync } from 'ng-set-state';
import { AttributeItemModel } from 'src/api-client.generated';
import { FormField } from 'src/app/lib/forms/form-descriptor';
import { FormFieldsBuilder } from 'src/app/lib/forms/form-fields-builder';
import { ApiService } from 'src/app/services/api-service.service';
import { PopupService } from 'src/app/services/popup-service';

type State = ComponentState<SetItemsEditorComponent>;
type NewState = ComponentStateDiff<SetItemsEditorComponent>;

type ItemsData = {[index: number]: string }&{[index: string]: string|null};

@Component({
  selector: 'app-set-items-editor',
  templateUrl: './set-items-editor.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SetItemsEditorComponent {

  constructor(readonly api: ApiService, readonly popupService: PopupService, cd: ChangeDetectorRef) {
    initializeStateTracking(this, {
      includeAllPredefinedFields: true,
      onStateApplied: () => cd.detectChanges()
    });
  }

  @Input()
  attributeId: string|null = null;

  @Output()
  readonly attributeIdChange = new EventEmitter<string|null>();

  @Emitter()
  result: boolean | null = null;

  @Output()
  readonly resultChange = new EventEmitter<boolean>();

  fields: FormField[] = [];

  initialData: ItemsData|null = null;

  currentData: ItemsData|null = null;

  savingData: ItemsData|null = null;

  isError = false;

  isVisited = false;

  @WithAsync('attributeId')
  static async loadItems(getState: () => State): Promise<NewState>{
    const s = getState();
    if (s.attributeId != null) {

      const items = await s.api.adminAttributeItemsGetById(s.attributeId);

      const data: ItemsData = items.reduce((acc, next, index) => {
          acc[index] = next.title;
          acc[index.toString() + '_id'] = next.id ?? null;
          return acc;
        },  {} as ItemsData);

      return {
        initialData: data,
        currentData: data
      };
    }
    return null;
  }

  @With('currentData')
  static rebuildForm(s: State): NewState{

    if (s.currentData == null){
      return {
        fields: s.fields.length === 0 ? s.fields : []
      };
    }

    const keys = Object.keys(s.currentData).filter(k => !k.toString().endsWith('_id'));

    if (keys.length !== s.fields.length) {
      return {
        fields: keys
          .reduce(
            (acc, next, index) => acc.addTextField({
                id: next,
                label: 'Item ' + (index + 1).toString(),
                maxLength: 255})
              .mandatory()
              .withTailButtons([
                {
                  iconId: 'icon-delete',
                  clickArgument: 'delete:' + next,
                  title: 'Delete Item'
                },
                {
                  iconId: 'icon-arrow-up',
                  clickArgument: 'up:' + next,
                  title: 'Up',
                  disabled: index === 0
                },
                {
                  iconId: 'icon-arrow-down',
                  clickArgument: 'down:' + next,
                  title: 'Down',
                  disabled: index === keys.length - 1
                }
              ]) ,
          new FormFieldsBuilder<any>()).done()
      };
    }
    return null;
  }

  @WithAsync('savingData').Finally(() => ({savingData: null}))
  static async save(getState: () => State): Promise<NewState>{
    const s = getState();

    if (s.savingData == null || s.attributeId == null){
      return null;
    }

    const keys = Object.keys(s.savingData);
    const items: AttributeItemModel[] = [];
    for (const key of keys){
      if (!key.toString().endsWith('_id')){
        const title = s.savingData[ key as any];
        const id = s.savingData[key.toString() + '_id'];
        items.push({id, title});
      }
    }

    try{
      await s.api.adminAttributeItemsPost(s.attributeId, items);

      return {
        result: true,
        attributeId: null
      };
    }
    catch (e){
      s.api.handleError(e);
    }
    return null;
  }

  onDialogButtonClick(buttonId: string): void{
    if (buttonId === 'ok') {
      if (!this.isError){
        this.savingData = this.currentData;
      }
      else{
        this.isVisited = true;
      }
    }
    else if (buttonId === 'cancel') {
      this.attributeId = null;
    }
    else {
      throw new Error(`Unknown button id: '${buttonId}'`);
    }
  }

  addNewItem(): void {
    const newData = this.currentData == null ? {} : Object.assign({}, this.currentData);
    const newKey = Object.keys(newData).map(Number).reduce((acc, next) => next > acc ? next : acc, 0) + 1;
    newData[newKey] = '';
    newData[newKey.toString()] = null;

    this.currentData = newData;
  }

  onTailButtonClick(arg: any): void{
    if (typeof arg === 'string'){
      const parts = arg.split(':');
      if (parts.length === 2){
        const action = parts[0];
        const index = Number.parseInt(parts[1]);
        if (!Number.isNaN(index)){
          switch (action){
            case 'delete':
              const newData = this.currentData == null ? {} : Object.assign({}, this.currentData);
              delete newData[index];
              this.currentData = newData;
              break;
            case 'up':
              if (this.currentData != null){
                const newDataUp = Object.assign({}, this.currentData);
                newDataUp[index]  = this.currentData[index - 1];
                newDataUp[index - 1]  = this.currentData[index];
                newDataUp[index.toString() + '_id']  = this.currentData[(index - 1).toString() + '_id'];
                newDataUp[(index - 1).toString() + '_id']  = this.currentData[index.toString() + '_id'];
                this.currentData = newDataUp;
              }
              break;
            case 'down':
              if (this.currentData != null){
                const newDataDown = Object.assign({}, this.currentData);
                newDataDown[index]  = this.currentData[index + 1];
                newDataDown[index + 1]  = this.currentData[index];
                newDataDown[index.toString() + '_id']  = this.currentData[(index + 1).toString() + '_id'];
                newDataDown[(index + 1).toString() + '_id']  = this.currentData[index.toString() + '_id'];
                this.currentData = newDataDown;
              }
              break;
          }

        }
      }
    }
  }
}
