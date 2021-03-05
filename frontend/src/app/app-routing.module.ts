import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { VodListContainerComponent } from './vod-list/vod-list-container/vod-list.component';

const routes: Routes = [
  {
    path: '', component: VodListContainerComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
