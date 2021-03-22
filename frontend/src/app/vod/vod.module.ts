import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { VodRoutingModule } from './vod-routing.module';
import { VodComponent } from './vod.component';
import { VodPlayerComponent } from './vod-player/vod-player.component';
import { SafePipe } from './safe.pipe';
import { VodChatComponent } from './vod-chat/vod-chat.component';


@NgModule({
  declarations: [VodComponent, VodPlayerComponent, SafePipe, VodChatComponent],
  imports: [
    CommonModule,
    VodRoutingModule
  ]
})
export class VodModule { }
