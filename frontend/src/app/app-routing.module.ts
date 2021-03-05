import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '', 
    loadChildren: () => import('./vod-list/vod-list.module').then(m => m.VodListModule)
  },
  { path: 'vods/:id', loadChildren: () => import('./vod/vod.module').then(m => m.VodModule) }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
