import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { VodComponent } from './vod.component';

const routes: Routes = [{ path: '', component: VodComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class VodRoutingModule { }
