import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { VodListComponent } from './vod-list/vod-list.component';

const routes: Routes = [
  {
    path: '', component: VodListComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
