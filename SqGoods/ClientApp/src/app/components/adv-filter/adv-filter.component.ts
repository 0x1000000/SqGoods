import { ChangeDetectionStrategy, ChangeDetectorRef, Component, EventEmitter, Input, Output } from '@angular/core';
import { ComponentState, ComponentStateDiff, initializeStateTracking, With } from 'ng-set-state';
import { FilterAndModel, FilterBoolModel, FilterOrModel } from 'src/api-client.generated';
import { FieldDescriptor } from 'src/app/lib/forms/form-descriptor';
import { EmptyAndPredicate, isAndOr, isEmptyPredicate, isPredicate, isValidBool, toAnd } from './filter-type-guards';

type State = ComponentState<AdvFilterComponent>;
type NewState = ComponentStateDiff<AdvFilterComponent>;

@Component({
  selector: 'app-adv-filter',
  templateUrl: './adv-filter.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdvFilterComponent {

  @Input()
  isOpen = false;

  @Input()
  fields: FieldDescriptor[]|null = null;

  @Input('expr')
  exprIn: FilterBoolModel|null = null;

  expr: FilterBoolModel|null = null;

  @Output()
  readonly isOpenChange = new EventEmitter<boolean>();

  @Output()
  readonly exprChange = new EventEmitter<FilterBoolModel|null>();

  internalExpr: FilterOrModel|FilterAndModel = EmptyAndPredicate;

  constructor(cd: ChangeDetectorRef){
    initializeStateTracking(this, {
      onStateApplied: () => cd.detectChanges(),
      includeAllPredefinedFields: true
    });
  }

  @With('exprIn')
  static onExprInput(s: State): NewState{
    return {
      internalExpr: AdvFilterComponent.wrap(s.exprIn),
      expr: s.exprIn
    };
  }

  static wrap(exprIn: FilterBoolModel|null): FilterOrModel|FilterAndModel{
    if (exprIn == null){
      return EmptyAndPredicate;
    }
    if (isPredicate(exprIn)){
      return toAnd([exprIn]);
    }
    else if (isAndOr(exprIn)){
      return exprIn;
    }
    else {
      throw new Error('Unknown filter type');
    }
  }

  onButtonClick(id: string): void{
    if (id === 'ok'){
      if (isValidBool(this.internalExpr)){
        if (isAndOr(this.internalExpr)
          && this.internalExpr.items.length === 1
          && isPredicate(this.internalExpr.items[0]))
        {
          this.expr = this.internalExpr.items[0];
        }
        else{
          this.expr = this.internalExpr;
        }
        this.isOpen = false;
      }
      else {
        if (this.internalExpr.items.length < 1
          || (
            this.internalExpr.items.length === 1
            &&
            isEmptyPredicate(this.internalExpr.items[0])))
        {
          this.expr = null;
          this.isOpen = false;
        }
      }
    }
    else{
      this.internalExpr = AdvFilterComponent.wrap(this.exprIn);
      this.isOpen = false;
    }
  }
}
