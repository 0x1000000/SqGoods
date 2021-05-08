import { ChangeDetectionStrategy, ChangeDetectorRef, Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Emitter, With, WithAsync } from 'ng-set-state';
import { ComponentState, ComponentStateDiff, initializeStateTracking } from 'ng-set-state';
import { AttributeListModel, AttributeSwapFormItemModel, AttributeSwapFormModel, CategoryListModel, SgAttributeType } from 'src/api-client.generated';
import { AttributeTypeDescriptor } from 'src/app/enum-descriptors';
import { ApiService } from 'src/app/services/api-service.service';
import { PopupService } from 'src/app/services/popup-service';

type State = ComponentState<EditAttributesComponent>;
type NewState = ComponentStateDiff<EditAttributesComponent>;

@Component({
  selector: 'app-edit-attributes',
  templateUrl: './edit-attributes.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EditAttributesComponent {

  constructor(
    readonly api: ApiService,
    readonly popUpService: PopupService,
    cd: ChangeDetectorRef,
    readonly router: Router,
    ar: ActivatedRoute)
  {
    initializeStateTracking(this, {
      onStateApplied: () => cd.detectChanges(),
      includeAllPredefinedFields: true });

    this.selectedCategoryId = null;

    ar.queryParamMap.subscribe(p => {
      const pStr = p.get('categoryId');
      if (pStr){
        if (pStr != null && pStr !== '') {
        this.selectedCategoryId = pStr;
        }
      }
    });
  }

  attributes: AttributeListModel[] = [];

  initialAttributes: AttributeListModel[] = [];

  swapForm: AttributeSwapFormModel|null = null;

  categories: CategoryListModel[] = [];

  selectedCategoryId: string | null;

  editingAttributeId: string | null = null;

  deletingAttributeId: string | null = null;

  editingSetItemAttributeId: string | null = null;

  addExistingCategoryId: string | null = null;

  @Emitter()
  refresh: null = null;

  @WithAsync('selectedCategoryId', 'refresh').OnConcurrentLaunchReplace()
  static async refresh(getState: () => State): Promise<NewState> {
    const state = getState();

    state.router.navigate(
      [],
      {
        queryParams: {categoryId: state.selectedCategoryId},
        queryParamsHandling: 'merge', // remove to replace all query params by provided
      });

    try{
      const r = await state.api.adminAttributesGetPage(0, 10, state.selectedCategoryId ?? undefined);

      return {
        attributes: r.items!,
        initialAttributes: r.items!,
        categories: r.categoryFilter!};
    }
    catch (e){
      state.api.handleError(e);
      return null;
    }
  }

  @WithAsync('deletingAttributeId').OnConcurrentLaunchPutAfter().Finally(() => ({deletingAttributeId: null}))
  static async delete(getState: () => State): Promise<NewState> {
    const s = getState();
    if (s.deletingAttributeId == null){
      return null;
    }

    if (!await s.popUpService.showConfirmation('Are you sure?')) {
      return null;
    }

    try{
      await s.api.adminAttributesDelete([s.deletingAttributeId]);
      s.popUpService.showToastMessage('Successfully deleted');

      return {
        refresh: null
      };
    }
    catch (e) {
      s.api.handleError(e);
      return null;
    }
  }

  @With('attributes', 'initialAttributes')
  static onReorder(s: State): NewState{
    if (s.selectedCategoryId == null || s.attributes == null || s.attributes.length < 1 || s.attributes === s.initialAttributes){
      return null;
    }
    if (s.attributes.length !== s.initialAttributes.length){
      throw new Error('Different length');
    }

    const diff: AttributeSwapFormItemModel[] = [];

    for (let i = 0; i < s.attributes.length; i++) {
      if (s.attributes[i].id !== s.initialAttributes[i].id) {
        diff.push({originalId: s.initialAttributes[i].id, currentId: s.attributes[i].id});
      }
    }
    if (diff.length === 0){
      return {
        swapForm: null
      };
    }

    return {
      swapForm: {categoryId: s.selectedCategoryId, swaps: diff }
    };
  }

  @WithAsync('swapForm').Debounce(2000/*Ms*/)
  static async sendSwapForm(getState: () => State): Promise<NewState>{
    const s = getState();

    if (s.swapForm == null){
      return null;
    }

    try
    {
      await s.api.adminAttributeSwapFormPost([s.swapForm]);
      s.popUpService.showToastMessage('Successfully reordered');
    }
    catch (e){
      s.api.handleError(e);
    }

    return {
      swapForm: null
    };
  }

  onOrderUp(a: AttributeListModel): void{
    const index = this.attributes.indexOf(a);
    this.attributes = Object.assign([], this.attributes);
    this.attributes[index] = this.attributes[index - 1];
    this.attributes[index - 1] = a;
  }

  onOrderDown(a: AttributeListModel): void{
    const index = this.attributes.indexOf(a);
    this.attributes = Object.assign([], this.attributes);
    this.attributes[index] = this.attributes[index + 1];
    this.attributes[index + 1] = a;
  }

  onAdd(): void{
    this.editingAttributeId = '';
  }

  onAddExisting(): void{
    if (this.selectedCategoryId != null){
      this.addExistingCategoryId = this.selectedCategoryId;
    }
  }

  onResult(res: boolean): void{
    if (res){
      this.refresh = null;
    }
  }

  getTypeText(a: AttributeListModel): string{
    return AttributeTypeDescriptor[a.type];
  }

  isEditItemsVisible(a: AttributeListModel): boolean{
    return a.type === SgAttributeType.Select || a.type === SgAttributeType.SubSet;
  }

  isOrderUpVisible(a: AttributeListModel): boolean{
    return this.attributes != null && this.attributes.length > 0 && this.attributes[0] !== a;
  }

  isOrderDownVisible(a: AttributeListModel): boolean{
    return this.attributes != null && this.attributes.length > 0 && this.attributes[this.attributes.length - 1] !== a;
  }
}
