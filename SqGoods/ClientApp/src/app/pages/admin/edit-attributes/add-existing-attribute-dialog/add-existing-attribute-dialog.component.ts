import { ChangeDetectionStrategy, ChangeDetectorRef, Component, EventEmitter, Input, Output } from '@angular/core';
import { ComponentState, ComponentStateDiff, Emitter, initializeStateTracking, WithAsync } from 'ng-set-state';
import { AttributeListModel } from 'src/api-client.generated';
import { ApiService } from 'src/app/services/api-service.service';

type State = ComponentState<AddExistingAttributeDialogComponent>;
type NewState = ComponentStateDiff<AddExistingAttributeDialogComponent>;

@Component({
  selector: 'app-add-existing-attribute-dialog',
  templateUrl: './add-existing-attribute-dialog.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AddExistingAttributeDialogComponent {

  constructor(readonly api: ApiService, cd: ChangeDetectorRef){
    initializeStateTracking(this, {
      onStateApplied: () => cd.detectChanges(),
      includeAllPredefinedFields: true
    });
  }

  @Input()
  public categoryId: string|null = null;

  @Output()
  public readonly categoryIdChange = new EventEmitter<string|null>();

  @Emitter()
  result: boolean | null = null;

  @Output()
  readonly resultChange = new EventEmitter<any>();

  public items: AttributeListModel[] | null = null;

  public readonly selected = new Set<string>();

  public savingIds: string[]|null = null;

  @WithAsync('categoryId').OnConcurrentLaunchReplace()
  static async loadAttributes(getState: () => State): Promise<NewState>{
    const s = getState();

    if (s.categoryId == null){
      return {
        items: null
      };
    }

    try {
      const r = await s.api.adminAttributesGetPage(0, 100, s.categoryId, true);

      return {
        items: r.items
      };

    } catch (e) {
      s.api.handleError(e);
    }
    return null;
  }

  @WithAsync('savingIds').OnConcurrentLaunchPutAfter().Finally(() => ({savingIds: null}))
  static async save(getState: () => State): Promise<NewState>{
    const s = getState();
    if (s.categoryId == null || s.savingIds == null || s.savingIds.length < 1){
      return null;
    }

    try {
      await s.api.adminCategoriesPostAttributes(s.categoryId, s.savingIds);
      return {
        categoryId: null,
        result: true
      };
    } catch (e) {
      s.api.handleError(e);
    }
    return null;

  }

  onCheckedChange(item: AttributeListModel, value: boolean): void {
    if (value){
      this.selected.add(item.id);
    }
    else{
      this.selected.delete(item.id);
    }
  }

  onDialogButtonClick(buttonId: string): void {
    if (buttonId === 'ok' && this.selected.size > 0){
      this.savingIds = Array.from(this.selected);
    }
    else{
      this.categoryId = null;
    }
  }
}
