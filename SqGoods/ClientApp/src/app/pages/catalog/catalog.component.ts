import { ChangeDetectionStrategy, ChangeDetectorRef, Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AsyncInit, ComponentState, ComponentStateDiff, initializeStateTracking, With, WithAsync } from 'ng-set-state';
import { FilterBoolModel, IdNameModel, ProductListModel } from 'src/api-client.generated';
import { isAndOr } from 'src/app/components/adv-filter/filter-type-guards';
import { FieldDescriptor } from 'src/app/lib/forms/form-descriptor';
import { ApiService } from 'src/app/services/api-service.service';

type State = ComponentState<CatalogComponent>;
type NewState = ComponentStateDiff<CatalogComponent>;

@Component({
  selector: 'app-catalog',
  templateUrl: './catalog.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CatalogComponent {

  categories: IdNameModel[] = [];

  products: ProductListModel[]|null = null;

  fields: FieldDescriptor[]|null = null;

  selectedCategoryId: string | null = null;

  isFilterOpen = false;

  filterExpr: FilterBoolModel|null = null;

  filterNodeCounter = 0;

  constructor(readonly api: ApiService, cd: ChangeDetectorRef, readonly router: Router, ar: ActivatedRoute){
    const t = initializeStateTracking<CatalogComponent>(this, {
      includeAllPredefinedFields: true,
      onStateApplied: () => cd.detectChanges()});

    ar.queryParamMap.subscribe(p => {

      let selectedCategoryId = this.selectedCategoryId;
      let filterExpr = this.filterExpr;

      const pStr = p.get('categoryId');
      if (pStr){
        if (pStr != null && pStr !== ''){
          selectedCategoryId = pStr;
        }
      }

      const filter = p.get('filter');
      if (filter && this.filterExpr == null){
        filterExpr = JSON.parse(decodeURIComponent(filter));
      }

      setTimeout(() => t.modifyStateDiff({selectedCategoryId, filterExpr}));
    });
  }

  @AsyncInit()
  static async init(getState: () => State): Promise<NewState>{
    const initialState = getState();
    const cat = await initialState.api.catalogGetTopCategories();

    const currentState = getState();

    let sId: string | null = null;
    if (cat.length > 0){
      if (currentState.selectedCategoryId != null && cat.some(c => c.id === currentState.selectedCategoryId)){
          sId = currentState.selectedCategoryId;

      } else{
        sId = cat[0].id;
      }
    }

    return {
      categories: cat,
      selectedCategoryId: sId
    };
  }

  @With('selectedCategoryId', 'filterExpr')
  static clearAttributes(s: State): NewState{
    s.router.navigate(
      [],
      {
        queryParams: {
          categoryId: s.selectedCategoryId,
          filter: s.filterExpr == null ? undefined : encodeURIComponent(JSON.stringify(s.filterExpr))
        },
        queryParamsHandling: 'merge', // remove to replace all query params by provided
      });
    return{
      products: null
    };
  }

  @With('selectedCategoryId')
  static cleanData(s: State, p: State): NewState{
    return {
      fields: null,
      filterExpr: p.selectedCategoryId == null ? s.filterExpr : null
    };
  }

  @With('filterExpr')
  static calcFilterNodeCounter(s: State): NewState{
    if (s.filterExpr == null){
      return {filterNodeCounter: 0};
    }

    return {filterNodeCounter: calcPredicates(s.filterExpr)};

    function calcPredicates(e: FilterBoolModel): number{
      if (isAndOr(e)){
        return e.items.reduce<number>((acc, next) => acc + calcPredicates(next), 0);
      }
      return 1;
    }
  }

  @WithAsync('selectedCategoryId', 'filterExpr').OnConcurrentLaunchReplace()
  static async loadProducts(getState: () => State): Promise<NewState>{
    const initialState = getState();

    if (initialState.selectedCategoryId == null) {
      return{
        products: null
      };
    }

    try {

      const filterJson = initialState.filterExpr == null ? undefined : JSON.stringify(initialState.filterExpr);

      const products = await initialState.api.catalogGetCategoryProductPage(initialState.selectedCategoryId, filterJson);
      return {
        products: products?.items
      };
    }
    catch (e){
      initialState.api.handleError(e);
      return null;
    }
  }

  @WithAsync('selectedCategoryId').OnConcurrentLaunchReplace()
  static async loadFields(getState: () => State): Promise<NewState>{
    const initialState = getState();

    if (initialState.selectedCategoryId == null) {
      return{
        fields: null
      };
    }

    try {
      const fields = await initialState.api.catalogGetCategoryAttributes(initialState.selectedCategoryId) as FieldDescriptor[];
      return {
        fields
      };
    }
    catch (e){
      initialState.api.handleError(e);
      return null;
    }
  }
}
