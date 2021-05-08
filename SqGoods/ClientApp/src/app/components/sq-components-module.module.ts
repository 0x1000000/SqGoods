import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppProgressIndicatorComponent } from './app-progress-indicator/app-progress-indicator.component';
import { AppPopupsComponent } from './app-popups/app-popups.component';
import { AppToastsComponentComponent } from './app-popups/app-toasts-component/app-toasts-component.component';
import { LibModule } from '../lib/lib.module';
import { RouterTabsComponent } from './router-tabs/router-tabs.component';
import { RouterModule } from '@angular/router';
import { AppMessageBoxComponent } from './app-popups/app-message-box/app-message-box.component';
import { AdvFilterComponent } from './adv-filter/adv-filter.component';
import { AdvFilterPredicateComponent } from './adv-filter/adv-filter-predicate/adv-filter-predicate.component';
import { AdvFilterAndOrComponent } from './adv-filter/adv-filter-and-or/adv-filter-and-or.component';
import { NumRangeComponent } from './adv-filter/adv-filter-predicate/num-range/num-range.component';
import { AdvFilterBoolComponent } from './adv-filter/adv-filter-bool/adv-filter-bool.component';

@NgModule({
  imports: [
    CommonModule,
    LibModule,
    RouterModule
  ],
  declarations: [
    AppProgressIndicatorComponent,
    AppPopupsComponent, AppToastsComponentComponent, AppMessageBoxComponent,
    RouterTabsComponent,
    AdvFilterComponent, AdvFilterPredicateComponent, AdvFilterAndOrComponent, AdvFilterBoolComponent,
    NumRangeComponent
  ],
  exports: [
    AppProgressIndicatorComponent,
    AppPopupsComponent,
    AdvFilterComponent,
    RouterTabsComponent, AdvFilterPredicateComponent, AdvFilterAndOrComponent
  ]
})
export class SqComponentsModuleModule { }
