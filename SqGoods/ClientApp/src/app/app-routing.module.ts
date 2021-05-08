import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminComponent } from './pages/admin/admin.component';
import { DbDataManagementComponent } from './pages/admin/db-data-management/db-data-management.component';
import { EditAttributesComponent } from './pages/admin/edit-attributes/edit-attributes.component';
import { EditCategoriesComponent } from './pages/admin/edit-categories/edit-categories.component';
import { EditProductsComponent } from './pages/admin/edit-products/edit-products.component';
import { CatalogComponent } from './pages/catalog/catalog.component';

const routes: Routes = [
  { path: '', redirectTo: 'catalog', pathMatch: 'full' },
  { path: 'catalog', component: CatalogComponent, data: {title: 'Catalog'} },
  { path: 'admin', component: AdminComponent, data: {title: 'Admin'}, children: [
    {path: '', redirectTo: 'categories', pathMatch: 'full'},
    {path: 'categories', component: EditCategoriesComponent, data: {title: 'Categories'}},
    {path: 'attributes', component: EditAttributesComponent, data: {title: 'Attributes'}},
    {path: 'products', component: EditProductsComponent, data: {title: 'Products'}},
    {path: 'db-management', component: DbDataManagementComponent, data: {title: 'Database Data'}}
  ] }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
