import { ChangeDetectorRef, Component, EventEmitter, Input, Output } from '@angular/core';
import { ComponentState, ComponentStateDiff, initializeStateTracking, With } from 'ng-set-state';
import { FilterAndModel, FilterBoolModel, FilterOrModel } from 'src/api-client.generated';
import { FieldDescriptor } from 'src/app/lib/forms/form-descriptor';
import { EmptyAnd, EmptyPredicate, isAndOr, isOr, isPredicate, isValidBool, isValidPredicate, toAnd, toOr } from 'src/app/components/adv-filter/filter-type-guards';

type State = ComponentState<AdvFilterAndOrComponent>;
type NewState = ComponentStateDiff<AdvFilterAndOrComponent>;

@Component({
  selector: 'app-adv-filter-and-or',
  templateUrl: './adv-filter-and-or.component.html'
})
export class AdvFilterAndOrComponent {

  @Input()
  fields: FieldDescriptor[]|null = null;

  @Input()
  expr: FilterAndModel|FilterOrModel = EmptyAnd;

  @Output()
  readonly exprChange = new EventEmitter<FilterAndModel|FilterOrModel>();

  isOr = false;

  isAddDisabled = false;

  items: FilterBoolModel[] = [];

  constructor(cd: ChangeDetectorRef){
    initializeStateTracking(this, {
      onStateApplied: () => cd.detectChanges(),
      includeAllPredefinedFields: true
    });
  }

  @With('expr')
  static onExpr(s: State): NewState{
    return {
      isOr: isOr(s.expr),
      items: isAndOr(s.expr) ? s.expr.items : []
    };
  }

  @With('items', 'isOr')
  static buildExpression(s: State): NewState{

    if (s.isOr){
      return {
        expr: toOr(s.items)
      };
    }
    else{
      return {
        expr: toAnd(s.items)
      };
    }
  }

  @With('items', 'expr')
  static calcIsAddDisabled(s: State): NewState{
    return {
      isAddDisabled: s.items.length > 0 && !isValidBool(s.expr)
    };
  }

  trackByIndex = (index: number, item: FilterBoolModel) => index;

  onAddClick(): void {
    const newItems = [...this.items];
    newItems.push(EmptyPredicate);
    this.items = newItems;
  }

  onRemoveClick(i: FilterBoolModel, index: number): void{
    const newItems = [...this.items];
    newItems.splice(index, 1);
    this.items = newItems;
  }

  onItemChange(i: FilterBoolModel, index: number): void {
    if (i === this.items[index]){
      return;
    }
    const newItems = [...this.items];
    newItems[index] = i;
    this.items = newItems;
  }

  isToSubVisible(i: FilterBoolModel): boolean{
    return isValidPredicate(i) && this.items.length > 1;
  }

  toSub(i: FilterBoolModel, index: number): void{
    const newItems = [...this.items];
    if (this.isOr){
      newItems[index] = toAnd([i]);
    }
    else{
      newItems[index] = toOr([i]);
    }
    this.items = newItems;
  }

  isFromSubVisible(i: FilterBoolModel): boolean{
    return isAndOr(i) && i.items.length === 1;
  }

  isAlertVisible(i: FilterBoolModel): boolean{
    return isPredicate(i) && !isValidPredicate(i);
  }

  fromSub(i: FilterBoolModel, index: number): void{
    const newItems = [...this.items];

    if (isAndOr(i) && i.items.length === 1){
      newItems[index] = i.items[0];
    }
    else{
      throw new Error('Unexpected item');
    }
    this.items = newItems;
  }
}
