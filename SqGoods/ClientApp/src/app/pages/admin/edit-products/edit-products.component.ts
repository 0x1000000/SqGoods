import { ChangeDetectionStrategy, ChangeDetectorRef, Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AsyncInit, ComponentState, ComponentStateDiff, Emitter, initializeStateTracking, With, WithAsync } from 'ng-set-state';
import { IdNameModel, ProductListModel } from 'src/api-client.generated';
import { ApiService } from 'src/app/services/api-service.service';

type State = ComponentState<EditProductsComponent>;
type NewState = ComponentStateDiff<EditProductsComponent>;

@Component({
  selector: 'app-edit-products',
  templateUrl: './edit-products.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EditProductsComponent {

  constructor(readonly api: ApiService, cd: ChangeDetectorRef, readonly router: Router, ar: ActivatedRoute) {
    initializeStateTracking(this, {
      onStateApplied: () => cd.detectChanges(),
      includeAllPredefinedFields: true });

    ar.queryParamMap.subscribe(p => {
        const pStr = p.get('categoryId');
        if (pStr){
          if (pStr != null && pStr !== '') {
          this.selectedCategoryId = pStr;
          }
        }
      });
  }

  categories: IdNameModel[] | null = null;

  selectedCategoryId: string | null = null;

  products: ProductListModel[]|null = null;

  @Emitter()
  refresh: any = null;

  productId: string| null = null;

  @AsyncInit()
  static async init(getState: () => State): Promise<NewState>{
    const s = getState();
    try{
      const categories = await s.api.adminProductsGetCategories();
      return {categories};
    }
    catch (e){
      s.api.handleError(e);
      return null;
    }
  }

  @With('selectedCategoryId')
  static onCategorySelect(s: State): NewState {
    s.router.navigate(
      [],
      {
        queryParams: {categoryId: s.selectedCategoryId},
        queryParamsHandling: 'merge', // remove to replace all query params by provided
      });
    return null;
  }

  @WithAsync('selectedCategoryId', 'refresh')
  static async loadProducts(getState: () => State): Promise<NewState> {
    const s = getState();

    if (s.selectedCategoryId == null){
      return {products: null};
    }

    try {
      const data = await s.api.adminProductsGetPage(0, 100, s.selectedCategoryId);

      return {
        products: data.items
      };
    }
    catch (e){
      s.api.handleError(e);
      return null;
    }
  }

  onAdd(): void{
    this.productId = '';
  }

  onEdit(productId: string): void {
    this.productId = productId;
  }

  onResult(result: boolean): void{
    if (result){
      this.refresh = null;
    }
  }
}
