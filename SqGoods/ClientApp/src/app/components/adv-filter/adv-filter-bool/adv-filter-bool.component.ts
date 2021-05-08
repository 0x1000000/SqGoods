import { ChangeDetectorRef, Component, EventEmitter, Input, Output } from '@angular/core';
import { ComponentState, ComponentStateDiff, initializeStateTracking, With } from 'ng-set-state';
import { FilterAndModel, FilterBoolModel, FilterOrModel, FilterPredicateModel } from 'src/api-client.generated';
import { FieldDescriptor } from 'src/app/lib/forms/form-descriptor';
import { EmptyPredicate, isAnd, isOr, isPredicate } from 'src/app/components/adv-filter/filter-type-guards';

type State = ComponentState<AdvFilterBoolComponent>;
type NewState = ComponentStateDiff<AdvFilterBoolComponent>;

@Component({
  selector: 'app-adv-filter-bool',
  templateUrl: './adv-filter-bool.component.html'
})
export class AdvFilterBoolComponent {

  @Input()
  fields: FieldDescriptor[]|null = null;

  @Input()
  expr: FilterBoolModel = EmptyPredicate;

  @Output()
  readonly exprChange = new EventEmitter<FilterBoolModel>();

  predicate: FilterPredicateModel|null = EmptyPredicate;

  andOrExpr: FilterAndModel|FilterOrModel|null = null;

  constructor(cd: ChangeDetectorRef){
    initializeStateTracking(this, {
      onStateApplied: () => cd.detectChanges(),
      includeAllPredefinedFields: true
    });
  }

  @With('predicate', 'andOrExpr')
  static buildExpr(s: State): NewState{
    if (s.predicate != null){
      return {
        expr: s.predicate
      };
    }
    if (s.andOrExpr != null){
      return {
        expr: s.andOrExpr
      };
    }
    throw new Error('Unknown expression type');
  }

  @With('expr')
  static onExpr(s: State): NewState{
    if (s.expr != null) {
      if ((isOr(s.expr) || isAnd(s.expr))) {
        return {
          predicate: null,
          andOrExpr: s.expr
        };
      }
      else if (isPredicate(s.expr)){
        return {
          predicate: s.expr,
          andOrExpr: null
        };
      }
      else {
        throw new Error('Unknown expression type');
      }
    }
    return {
      predicate: null,
      andOrExpr: null
    };
  }
}
