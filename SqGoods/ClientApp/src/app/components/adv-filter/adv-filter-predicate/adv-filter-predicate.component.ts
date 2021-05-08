import { ChangeDetectionStrategy, ChangeDetectorRef, Component, EventEmitter, Input, Output } from '@angular/core';
import { ComponentState, ComponentStateDiff, initializeStateTracking, With } from 'ng-set-state';
import { FilterPredicateModel } from 'src/api-client.generated';
import { BooleanFieldDescriptor, FieldDescriptor, NumericFieldDescriptor, SelectStaticFieldDescriptor } from 'src/app/lib/forms/form-descriptor';
import { deepEqual } from 'src/app/lib/helpers';

import {
  EmptyPredicate,
  isBoolPredicate,
  isEmptyPredicate,
  isErrorPredicate,
  isIntBetweenPredicate,
  isPredicate,
  isSelectPredicate,
  isSetPredicate,
  toBoolPredicate,
  toErrorPredicate,
  toIntBetweenPredicate,
  toSelectPredicate,
  toSetPredicate
} from './../filter-type-guards';

import { isNumRange, toNumRange } from './num-range/num-range.component';

type State = ComponentState<AdvFilterPredicateComponent>;
type NewState = ComponentStateDiff<AdvFilterPredicateComponent>;

type Descriptors = {
  select?: SelectStaticFieldDescriptor,
  bool?: BooleanFieldDescriptor,
  num?: NumericFieldDescriptor,
};

@Component({
  selector: 'app-adv-filter-predicate',
  templateUrl: './adv-filter-predicate.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdvFilterPredicateComponent {

  public readonly boolItems = [{value: true, title: 'Yes'}, {value: false, title: 'No'}];

  @Input()
  fields: FieldDescriptor[]|null = null;

  @Input()
  predicate: FilterPredicateModel = EmptyPredicate;

  @Output()
  predicateChange = new EventEmitter<FilterPredicateModel|null>();

  selectedField: FieldDescriptor|null = null;

  descriptor: Descriptors = {};

  value: any = null;

  isError = false;

  isEmpty = false;

  constructor(cd: ChangeDetectorRef){
    initializeStateTracking(this, {
      onStateApplied: () => cd.detectChanges(),
      includeAllPredefinedFields: true
    });
  }

  @With('predicate', 'fields')
  static analyzePredicate(s: State): NewState{
    if (!isPredicate(s.predicate)){
      throw new Error('Only predicates are expected here');
    }

    if (s.fields == null || isEmptyPredicate(s.predicate)){
      return {
        isEmpty: true,
        isError: false,
        selectedField: null,
        value: null
      };
    }

    const selectedField = s.fields.find(f => f.id === s.predicate.attributeId);
    if (selectedField == null){
      return {
        predicate: EmptyPredicate
      };
    }

    if (isErrorPredicate(s.predicate)){
      return {
        isEmpty: false,
        isError: true,
        selectedField,
        value: null
      };
    }

    return {
      isEmpty: false,
      isError: false,
      selectedField,
      value: Helpers.extractValue(s.predicate)
    };
  }

  @With('selectedField')
  static onSelectedChanged(s: State): NewState{

    return {
      value: s.selectedField != null && s.selectedField.id === s.predicate.attributeId
        ? Helpers.extractValue(s.predicate)
        : null,
      descriptor: Helpers.descriptorByField(s.selectedField)
    };
  }

  @With('descriptor', 'value')
  static buildPredicate(s: State): NewState{
    return {
      predicate: Helpers.buildPredicate(s.descriptor, s.value, s.predicate),
    };
  }
}

class Helpers{

  static extractValue(predicate: FilterPredicateModel): any{
    if (isSelectPredicate(predicate)){
      return predicate.selectValueId;
    }
    if (isSetPredicate(predicate)){
      return predicate.selectValueIds;
    }
    if (isBoolPredicate(predicate)){
      return predicate.value;
    }
    if (isIntBetweenPredicate(predicate)){
      return toNumRange(predicate.from ?? null, predicate.to ?? null);
    }
    return null;
  }

  static descriptorByField(selectedField: FieldDescriptor|null): Descriptors{
    const newDescriptor: Descriptors = {};

    if (selectedField != null ){
      if (selectedField.type === 'select-static'){
        newDescriptor.select = selectedField;
      }
      else if (selectedField.type === 'numeric'){
        newDescriptor.num = selectedField;
      }
      else if (selectedField.type === 'bool'){
        newDescriptor.bool = selectedField;
      }
      else{
        throw new Error(`Not supported descriptor field ${selectedField.type}`);
      }
    }
    return newDescriptor;
  }

  static buildPredicate(descriptor: Descriptors, value: any, oldPredicate: FilterPredicateModel): FilterPredicateModel{
    let predicate: FilterPredicateModel = EmptyPredicate;

    if (descriptor.select != null) {
      predicate = Helpers.caseSelect(descriptor.select, value);
    }
    else if (descriptor.num != null) {
      predicate = Helpers.caseNum(descriptor.num, value);
    }
    else if (descriptor.bool != null) {
      predicate = Helpers.caseBool(descriptor.bool, value);
    }

    if (deepEqual(predicate, oldPredicate)){
      predicate = oldPredicate;
    }
    return predicate;
  }

  private static caseSelect(descriptor: SelectStaticFieldDescriptor, value: any): FilterPredicateModel{
    if (descriptor.multi){
      if (!Array.isArray(value)){
        return toErrorPredicate(descriptor.id);
      }
      return toSetPredicate({selectValueIds: value, attributeId: descriptor.id });
    }
    else {
      if (typeof value !== 'string'){
        return toErrorPredicate(descriptor.id);
      }
      else{
        return toSelectPredicate({selectValueId: value, attributeId: descriptor.id });
      }
    }
  }

  private static caseNum(descriptor: NumericFieldDescriptor, value: any): FilterPredicateModel{
    if (!isNumRange(value)){
      return toErrorPredicate(descriptor.id);
    }
    return toIntBetweenPredicate({attributeId: descriptor.id, from: value.from, to: value.to});
  }

  private static caseBool(descriptor: BooleanFieldDescriptor, value: any): FilterPredicateModel{
    if (typeof value !== 'boolean'){
      return toErrorPredicate(descriptor.id);
    }
    return toBoolPredicate({attributeId: descriptor.id, value});
  }
}
