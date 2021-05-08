import { ChangeDetectionStrategy, ChangeDetectorRef, Component, EventEmitter, Input, Output } from '@angular/core';
import { ComponentState, ComponentStateDiff, Emitter, initializeStateTracking, With, WithAsync } from 'ng-set-state';
import { ProductUpdateModel } from 'src/api-client.generated';
import { FormField } from 'src/app/lib/forms/form-descriptor';
import { FormFieldsBuilder } from 'src/app/lib/forms/form-fields-builder';
import { ApiService } from 'src/app/services/api-service.service';

type State = ComponentState<EditProductDialogComponent>;
type NewState = ComponentStateDiff<EditProductDialogComponent>;

type FormData = {[attributeId: string]: string|string[]|number|boolean|null} & {
  __id: string,
  __name?: string,
  __imageUrl?: string
  __categoryId: string
};

@Component({
  selector: 'app-edit-product-dialog',
  templateUrl: './edit-product-dialog.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EditProductDialogComponent {

  constructor(readonly api: ApiService, cd: ChangeDetectorRef) {
    initializeStateTracking(this, {
      includeAllPredefinedFields: true,
      onStateApplied: () => cd.detectChanges()});
  }

  @Input()
  productId: string|null = null;

  @Input()
  categoryId: string|null = null;

  @Output()
  readonly productIdChange  = new EventEmitter<string|null>();

  @Emitter()
  result: boolean | null = null;

  @Output()
  readonly resultChange = new EventEmitter<any>();

  isOpen = false;

  fields: FormField[]|null = null;

  initialFormData: FormData|null = null;

  formData: FormData|null = null;

  isError = false;

  isVisited = false;

  savingData: ProductUpdateModel|null = null;

  @With('productId', 'categoryId')
  static calcIsOpen(s: State): NewState{
    return { isOpen: s.productId != null && s.categoryId != null };
  }

  @WithAsync('categoryId').OnConcurrentLaunchReplace()
  static async loadFields(getState: () => State): Promise<NewState>{
    const s = getState();
    if (s.categoryId == null){
      return {fields: null};
    }
    try{
      const fields = await s.api.adminCategoriesGetFields(s.categoryId);

      const result = new FormFieldsBuilder<FormData>()
        .addTextField({id: '__name', label: 'Name' }).mandatory()
        .addTextField({id: '__imageUrl', label: 'Image Url' })
        .done();

      result.push(... fields as FormField[]);

      return {fields: result, formData: {__id: '', __categoryId: s.categoryId} };
    }
    catch (e){
      s.api.handleError(e);
    }
    return null;
  }

  @WithAsync('productId', 'fields')
  static async loadData(getState: () => State): Promise<NewState>{

    const s = getState();

    if (s.productId == null || s.fields == null){
      return {
        initialFormData: null,
        formData: null
      };
    }
    if (s.productId === ''){
      return {
        initialFormData: null,
        formData: null
      };
    }

    try{
      const res = await s.api.adminProductsGet(s.productId);
      const fd = Helpers.toFormData(res, s.fields);
      return {
        initialFormData: fd,
        formData: fd
      };
    }
    catch (e){
      s.api.handleError(e);
      return null;
    }
  }

  @WithAsync('savingData').Finally(() => ({savingData: null}))
  static async save(getState: () => State): Promise<NewState>{
    const s = getState();
    if (s.savingData == null){
      return null;
    }

    try{
      if (s.savingData.id === ''){
        await s.api.adminProductsPost([s.savingData]);
      }
      else{
        await s.api.adminProductsPut([s.savingData]);
      }
      return {
        productId: null,
        result: true
      };
    }
    catch (e){
      s.api.handleError(e);
      return null;
    }

  }

  onButtonClick(buttonId: string): void{
    if (buttonId === 'ok') {
      if (this.isError || this.formData == null){
        this.isVisited = true;
      }
      else{
        this.savingData = Helpers.formFormObject(this.formData);
      }
    }
    else {
      this.productId = null;
    }
  }

  onFormError(isError: boolean): void{
    this.isError = isError;
  }
}

class Helpers{
  static formFormObject(form: FormData): ProductUpdateModel {
    const result: ProductUpdateModel = {
      id: form.__id,
      name: form.__name!,
      imageUrl: form.__imageUrl!,
      categoryId: form.__categoryId,
      values: []
    };

    const keys = Object.keys(form);
    for (const k of keys){
      if (!k.startsWith('__')){

        const val = form[k];
        const strVal = Array.isArray(val) ? val.join(';') : val?.toString();

        result.values.push({attributeId: k, value: strVal});
      }
    }
    return result;
  }

  static toFormData(model: ProductUpdateModel, formFields: FormField[]): FormData{
    const formData: FormData = {
      __id: model.id,
      __categoryId: model.categoryId,
      __name: model.name,
      __imageUrl: model.imageUrl
    };

    const numericFields = new Set<string>(formFields.filter(ff => ff.descriptor.type === 'numeric').map(ff => ff.descriptor.id));
    const setFields = new Set<string>(formFields.filter(ff => ff.descriptor.type === 'select-static' && ff.descriptor.multi).map(ff => ff.descriptor.id));
    const boolFields = new Set<string>(formFields.filter(ff => ff.descriptor.type === 'bool').map(ff => ff.descriptor.id));

    if (model.values != null){
      for (const v of model.values) {

        let value: string|string[]|null|number|boolean = v.value ?? null;

        if (value != null && numericFields.has(v.attributeId)){
          value = Number.parseInt(value);
          if (Number.isNaN(value)){
            value = null;
          }
        }
        if (value != null && boolFields.has(v.attributeId)){
          if (typeof value === 'string'){
            value = value === 'True' || value === 'true';
          }
        }
        if (typeof value === 'string' && setFields.has(v.attributeId)){
          value = value.split(';');
        }
        formData[v.attributeId] = value;
      }
    }

    return formData;
  }
}
