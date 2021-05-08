import {FilterBoolModel, FilterAndModel, FilterOrModel, FilterPredicateModel, FilterSelectPredicateModel, FilterSetPredicateModel, FilterBoolPredicateModel, FilterIntBetweenPredicateModel } from 'src/api-client.generated';

export const EmptyPredicate: FilterEmptyPredicateModel = {attributeId: '', type: 'empty-predicate'};

export const EmptyAnd: FilterAndModel = {type: 'and', items: []};

export const EmptyAndPredicate: FilterAndModel = {type: 'and', items: [EmptyPredicate]};

export function isValidBool(model: FilterBoolModel|null): boolean{
  if (model == null){
    return false;
  }
  if (isValidPredicate(model)){
    return true;
  }
  if (isAndOr(model)){
    return model.items != null && model.items.length > 0 && model.items.every(m => isValidBool(m));
  }
  return false;
}

export function isAnd(model: FilterBoolModel): model is FilterAndModel{
  return (model as FilterAndModel)?.type === 'and';
}

export function toAnd(items: FilterBoolModel[]): FilterAndModel{
  return {
    items,
    type: 'and'
  };
}

export function isOr(model: FilterBoolModel): model is FilterOrModel{
  return (model as FilterOrModel)?.type === 'or';
}

export function toOr(items: FilterBoolModel[]): FilterOrModel{
  return {
    items,
    type: 'or'
  };
}

export function isAndOr(model: FilterBoolModel): model is (FilterAndModel|FilterOrModel){
  return isAnd(model) || isOr(model);
}

export function isPredicate(model: FilterBoolModel): model is FilterPredicateModel{
  return isSelectPredicate(model)
    || isSetPredicate(model)
    || isIntBetweenPredicate(model)
    || isBoolPredicate(model)
    || isEmptyPredicate(model)
    || isErrorPredicate(model);
}

export function isValidPredicate(model: FilterBoolModel): model is FilterPredicateModel{
  return isPredicate(model) && !isEmptyPredicate(model) && !isErrorPredicate(model);
}

// FilterEmptyPredicateModel
export interface FilterEmptyPredicateModel extends FilterPredicateModel{
  type: 'empty-predicate';
}

export function isEmptyPredicate(model: FilterBoolModel|null): model is FilterEmptyPredicateModel {
  return (model as FilterEmptyPredicateModel)?.type === 'empty-predicate';
}

// FilterErrorPredicateModel
export interface FilterErrorPredicateModel extends FilterPredicateModel{
  type: 'error-predicate';
}

export function isErrorPredicate(model: FilterBoolModel|null): model is FilterErrorPredicateModel {
  return (model as FilterErrorPredicateModel)?.type === 'error-predicate';
}

export function toErrorPredicate(attributeId: string): FilterErrorPredicateModel {
  return {attributeId, type: 'error-predicate'};
}

// FilterSelectPredicateModel
export function isSelectPredicate(model: FilterBoolModel): model is FilterSelectPredicateModel{
  return (model as FilterSelectPredicateModel)?.type === 'select';
}

export function toSelectPredicate(model: Omit<FilterSelectPredicateModel, 'type'>): FilterSelectPredicateModel{
  return addTypeTag(model, 'select');
}

export function isSetPredicate(model: FilterBoolModel): model is FilterSetPredicateModel{
  return (model as FilterSetPredicateModel)?.type === 'set';
}

export function toSetPredicate(model: Omit<FilterSetPredicateModel, 'type'>): FilterSetPredicateModel{
  return addTypeTag(model, 'set');
}

export function isBoolPredicate(model: FilterBoolModel): model is FilterBoolPredicateModel{
  return (model as FilterBoolPredicateModel)?.type === 'bool';
}

export function toBoolPredicate(model: Omit<FilterBoolPredicateModel, 'type'>): FilterBoolPredicateModel{
  return addTypeTag(model, 'bool');
}

export function isIntBetweenPredicate(model: FilterBoolModel): model is FilterIntBetweenPredicateModel{
  return (model as FilterBoolPredicateModel)?.type === 'num_between';
}

export function toIntBetweenPredicate(model: Omit<FilterIntBetweenPredicateModel, 'type'>): FilterIntBetweenPredicateModel{
  return addTypeTag(model, 'num_between');
}


function addTypeTag<T extends {type: string}>(model: Omit<T, 'type'>, tagValue: string): T{
  const result = model as T;
  result.type = tagValue;
  return result;
}
