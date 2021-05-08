import { ChangeDetectionStrategy, ChangeDetectorRef, Component, EventEmitter, Input, Output } from '@angular/core';
import { ComponentState, ComponentStateDiff, Emitter, IncludeInState, initializeStateTracking, With, WithAsync } from 'ng-set-state';
import { CategoryUpdateModel } from 'src/api-client.generated';
import { FormField } from 'src/app/lib/forms/form-descriptor';
import { FormFieldsBuilder } from 'src/app/lib/forms/form-fields-builder';
import { ApiService } from 'src/app/services/api-service.service';
import { PopupService } from 'src/app/services/popup-service';

type State = ComponentState<CategoryEditDialogComponent>;
type NewState = ComponentStateDiff<CategoryEditDialogComponent>;

type FormModel = CategoryUpdateModel;

@Component({
  selector: 'app-category-edit-dialog',
  templateUrl: './category-edit-dialog.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CategoryEditDialogComponent {

  constructor(
    api: ApiService,
    popupService: PopupService,
    cd: ChangeDetectorRef)
  {
    this.api = api;
    this.popupService = popupService;

    initializeStateTracking<CategoryEditDialogComponent>(this, { onStateApplied: (s) => {
      cd.detectChanges();
    }});

    this.fields = new FormFieldsBuilder<CategoryUpdateModel>()
      .addTextField({id: 'name', label: 'Name', maxLength: 255})
        .mandatory()
      .addNumericField({id: 'order', label: 'Order'})
        .mandatory()
      .addNumericField({id: 'topOrder', label: 'Top order', note: 'If the top order is defined then the category will be visible in catalog top header'})
      .done();
  }

  @IncludeInState()
  readonly api: ApiService;

  @IncludeInState()
  readonly popupService: PopupService;

  readonly fields: FormField[];

  @Input()
  categoryId: string | null = null;

  @Output()
  readonly categoryIdChange = new EventEmitter<string|null>();

  @IncludeInState()
  @Emitter()
  result: boolean | null = null;

  @Output()
  readonly resultChange = new EventEmitter<any>();

  initialCategoryData: FormModel|null = null;

  categoryData: FormModel|null = null;

  savingCategoryData: FormModel|null = null;

  isError = false;

  isVisited = false;

  @WithAsync('categoryId')
  static async load(getState: () => State): Promise<NewState>{
    const initialState = getState();

    if (initialState.categoryId == null){
      return {categoryData: null};
    }

    if (initialState.categoryId === ''){
      return {categoryData: {
        id: initialState.categoryId,
        name: '',
        order: 0,
        topOrder: null
      }};
    }

    try{
      const categoryData: FormModel = await initialState.api.adminCategoriesGetById(initialState.categoryId);
      const currentState = getState();
      if (currentState.categoryId === initialState.categoryId){
        return {categoryData, initialCategoryData: categoryData };
      }
    }
    catch (e){
      initialState.api.handleError(e);
    }
    return null;
  }

  @WithAsync('savingCategoryData').Finally(() => ({savingCategoryData: null}))
  static async save(getState: () => State): Promise<NewState>{
    const initialState = getState();
    if (initialState.savingCategoryData == null){
      return null;
    }

    try{
      const data = initialState.savingCategoryData;
      if (data.id != null && data.id !== ''){
        await initialState.api.adminCategoriesPut([{
          id: data.id,
          name: data.name,
          order: data.order,
          topOrder: data.topOrder
          }]);
        initialState.popupService.showToastMessage('Successfully updated!');
      }
      else{
        await initialState.api.adminCategoriesPost([{
          name: data.name,
          order: data.order,
          topOrder: data.topOrder
          }]);
        initialState.popupService.showToastMessage('Successfully added!');
      }
      return {
        result: true,
        categoryId: null
      };
    }
    catch (e) {
      initialState.api.handleError(e);
    }
    return null;
  }

  onDialogButtonClick(buttonId: string): void{
    if (buttonId === 'ok') {
      if (this.categoryData != null && !this.isError) {
        this.savingCategoryData = this.categoryData;
      }
      else {
        this.isVisited = true;
      }
    }
    else if (buttonId === 'cancel') {
      this.result = false;
      this.categoryId = null;
    }
    else {
      throw new Error(`Unknown button id: '${buttonId}'`);
    }
  }

  onFormError(isError: boolean): void{
    this.isError = isError;
  }
}
