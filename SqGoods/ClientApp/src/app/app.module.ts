import { ErrorHandler } from '@angular/core';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppErrorHandler } from './app-error-handler';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SqComponentsModuleModule } from './components/sq-components-module.module';
import { LibModule } from './lib/lib.module';
import { AdminComponent } from './pages/admin/admin.component';
import { DbDataManagementComponent } from './pages/admin/db-data-management/db-data-management.component';
import { AddExistingAttributeDialogComponent } from './pages/admin/edit-attributes/add-existing-attribute-dialog/add-existing-attribute-dialog.component';
import { AttributeEditDialogComponent } from './pages/admin/edit-attributes/attribute-edit-dialog/attribute-edit-dialog.component';
import { EditAttributesComponent } from './pages/admin/edit-attributes/edit-attributes.component';
import { SetItemsEditorComponent } from './pages/admin/edit-attributes/set-items-editor/set-items-editor.component';
import { CategoryEditDialogComponent } from './pages/admin/edit-categories/category-edit-dialog/category-edit-dialog.component';
import { EditCategoriesComponent } from './pages/admin/edit-categories/edit-categories.component';
import { EditProductDialogComponent } from './pages/admin/edit-products/edit-product-dialog/edit-product-dialog.component';
import { EditProductsComponent } from './pages/admin/edit-products/edit-products.component';
import { CatalogComponent } from './pages/catalog/catalog.component';
import { ApiService } from './services/api-service.service';
import { PopupService, PopupServiceImpl } from './services/popup-service';

@NgModule({
  declarations: [
    AppComponent,
    CatalogComponent,
    AdminComponent,
    EditCategoriesComponent, CategoryEditDialogComponent,
    EditAttributesComponent, AttributeEditDialogComponent, SetItemsEditorComponent, AddExistingAttributeDialogComponent,
    EditProductsComponent, EditProductDialogComponent,
    DbDataManagementComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    LibModule,
    SqComponentsModuleModule,
    BrowserAnimationsModule
  ],
  providers: [ApiService, {provide: PopupService, useClass: PopupServiceImpl}, {provide: ErrorHandler, useClass: AppErrorHandler}],
  bootstrap: [AppComponent]
})
export class AppModule { }
