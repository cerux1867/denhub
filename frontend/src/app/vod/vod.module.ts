import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { VodRoutingModule } from './vod-routing.module';
import { VodComponent } from './vod.component';


@NgModule({
  declarations: [VodComponent],
  imports: [
    CommonModule,
    VodRoutingModule
  ]
})
export class VodModule { }
