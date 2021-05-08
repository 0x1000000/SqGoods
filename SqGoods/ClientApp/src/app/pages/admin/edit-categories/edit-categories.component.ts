import { ChangeDetectionStrategy, ChangeDetectorRef, Component } from '@angular/core';
import { ComponentState, ComponentStateDiff, Emitter, initializeStateTracking, WithAsync } from 'ng-set-state';
import { CategoryListModel } from 'src/api-client.generated';
import { ApiService } from 'src/app/services/api-service.service';
import { PopupService } from 'src/app/services/popup-service';

type State = ComponentState<EditCategoriesComponent>;
type NewState = ComponentStateDiff<EditCategoriesComponent>;

@Component({
  selector: 'app-edit-categories',
  templateUrl: './edit-categories.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EditCategoriesComponent {

  constructor(readonly api: ApiService, readonly popUpService: PopupService, cd: ChangeDetectorRef) {
    initializeStateTracking(this, {
      onStateApplied: () => cd.detectChanges(),
      includeAllPredefinedFields: true });

    this.refresh = null;
  }

  categories: CategoryListModel[] = [];

  editingCategoryId: string|null = null;

  deletingCategoryId: string|null = null;

  @Emitter()
  refresh: any = null;

  @WithAsync('refresh').OnConcurrentLaunchReplace()
  static async init(getState: () => State): Promise<NewState> {
    const s = getState();

    try{
      const r = await s.api.adminCategoriesGetPage(0, 10);
      return {categories: r.items!};
    }
    catch (e){
      s.api.handleError(e);
      return null;
    }
  }

  @WithAsync('deletingCategoryId').OnConcurrentLaunchPutAfter().Finally(() => ({deletingCategoryId: null}))
  static async delete(getState: () => State): Promise<NewState> {
    const s = getState();
    if (s.deletingCategoryId == null){
      return null;
    }

    if (!await s.popUpService.showConfirmation('Are you sure?')) {
      return null;
    }

    try{

      await s.api.adminCategoriesDelete([s.deletingCategoryId]);
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

  onAdd(): void {
    this.editingCategoryId = '';
  }

  onEditCategoryResult(result: boolean): void{
    if (result){
      this.refresh = null;
    }
  }
}
