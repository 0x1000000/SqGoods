import { ChangeDetectionStrategy, ChangeDetectorRef, Component, EventEmitter, Input, Output } from '@angular/core';
import { ComponentState, ComponentStateDiff, Emitter, initializeStateTracking, With, WithAsync } from 'ng-set-state';
import { AttributeUpdateModel, CategoryListModel } from 'src/api-client.generated';
import { AttributeTypeDescriptor } from 'src/app/enum-descriptors';
import { FormField } from 'src/app/lib/forms/form-descriptor';
import { FormFieldsBuilder } from 'src/app/lib/forms/form-fields-builder';
import { ApiService } from 'src/app/services/api-service.service';
import { PopupService } from 'src/app/services/popup-service';

type State = ComponentState<AttributeEditDialogComponent>;
type NewState = ComponentStateDiff<AttributeEditDialogComponent>;

type FormModel = AttributeUpdateModel;

@Component({
  selector: 'app-attribute-edit-dialog',
  templateUrl: './attribute-edit-dialog.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AttributeEditDialogComponent {

  constructor(readonly api: ApiService, readonly popupService: PopupService, cd: ChangeDetectorRef) {
    initializeStateTracking<AttributeEditDialogComponent>(this, {
      includeAllPredefinedFields: true,
      onStateApplied: () => cd.detectChanges()});
    this.api = api;

  }

  private static readonly Types = Object
    .keys(AttributeTypeDescriptor)
    .map(k => ({id: Number.parseInt(k), name:  AttributeTypeDescriptor[k] as string}));

  @Input()
  attributeId: string | null = null;

  @Input()
  categories: CategoryListModel[] = [];

  @Input()
  defaultCategoryId: string|null = null;

  @Output()
  readonly attributeIdChange = new EventEmitter<string|null>();

  @Emitter()
  result: boolean | null = null;

  @Output()
  readonly resultChange = new EventEmitter<any>();

  fields: FormField[];

  initialAttributeData: FormModel|null = null;

  attributeData: FormModel|null = null;

  savingAttributeData: FormModel|null = null;

  isVisited = false;

  isError = false;

  @With('categories')
  static createFormFields(state: State): NewState{
    const fields = new FormFieldsBuilder<FormModel>()
    .addTextField({id: 'name', label: 'Name', maxLength: 255})
      .mandatory()
    .addSelectField({id: 'categories', label: 'Categories', items: state.categories, multi: true, idMember: 'id', textMember: 'name'})
      .mandatory()
    .addSelectField({id: 'type', label: 'Type', items: AttributeEditDialogComponent.Types, idMember: 'id', textMember: 'name'})
      .mandatory()
    .addTextField({id: 'unit', label: 'Unit', maxLength: 255, emptyToNull: true})
    .addBoolField({id: 'mandatory', label: 'Mandatory'})
    .done();

    return {fields};
  }

  @WithAsync('attributeId')
  static async openLoadData(getState: () => State): Promise<NewState>{
    const initialState = getState();

    if (initialState.attributeId == null){
      return {
        attributeData: null,
        isVisited: false,
        isError: false,
        initialAttributeData: null
      };
    }

    if (initialState.attributeId === ''){
      return {attributeData: {
        id: initialState.attributeId,
        name: '',
        categories: initialState.defaultCategoryId == null ? [] : [initialState.defaultCategoryId],
        type: 0,
        mandatory: false
      }};
    }

    try{
      const attributeData: FormModel = await initialState.api.adminAttributesGetById(initialState.attributeId);
      const currentState = getState();
      if (currentState.attributeId === initialState.attributeId){
        return {attributeData, initialAttributeData: attributeData };
      }
    }
    catch (e){
      initialState.api.handleError(e);
    }
    return null;
  }

  @WithAsync('savingAttributeData').Finally(() => ({savingAttributeData: null}))
  static async save(getState: () => State): Promise<NewState>{
    const initialState = getState();
    if (initialState.savingAttributeData == null){
      return null;
    }

    const formData = initialState.savingAttributeData;

    try{

      if (formData.id != null && formData.id !== ''){
        await initialState.api.adminAttributesPut([{
          id: formData.id,
          name: formData.name,
          type: formData.type,
          categories: formData.categories,
          unit: formData.unit,
          mandatory: formData.mandatory
          }]);

        initialState.popupService.showToastMessage('Successfully updated!');
      }
      else{
        await initialState.api.adminAttributesPost([{
          name: formData.name,
          type: formData.type,
          categories: formData.categories,
          unit: formData.unit,
          mandatory: formData.mandatory}]);

        initialState.popupService.showToastMessage('Successfully added!');
      }

      return {
        attributeId: null,
        result: true
      };
    }
    catch (e) {
      initialState.api.handleError(e);
    }
    return null;
  }

  onDialogButtonClick(buttonId: string): void{
    if (buttonId === 'ok') {
      if (this.attributeData != null && !this.isError) {
        this.savingAttributeData = this.attributeData;
      }
      else {
        this.isVisited = true;
      }
    }
    else if (buttonId === 'cancel') {
      this.attributeId = null;
      this.result = false;
    }
    else {
      throw new Error(`Unknown button id: '${buttonId}'`);
    }
  }

  onFormError(isError: boolean): void{
    this.isError = isError;
  }
}
