import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VodListRoutingModule } from './vod-list-routing.module';
import { VodListContainerComponent } from './vod-list-container/vod-list.component';
import { VodCardComponent } from './vod-card/vod-card.component';
import { HumaniseLengthPipe } from './humanise-length.pipe';
import { HumaniseDateAgo } from './humanise-date-ago.pipe';
import { HumaniseViewsPipe } from './humanise-views.pipe';
import { LazyImgDirective } from './lazy-img.directive';
import { HttpClientModule } from '@angular/common/http';
import { NgxBootstrapIconsModule, twitch } from 'ngx-bootstrap-icons';
import { TransformTwitchThumbnailUrlPipe } from './transform-twitch-thumbnail-url.pipe';
import { VodsService } from './vods.service';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [
    HumaniseLengthPipe,
    HumaniseDateAgo,
    HumaniseViewsPipe,
    VodListContainerComponent,
    VodCardComponent,
    VodCardComponent,
    LazyImgDirective,
    TransformTwitchThumbnailUrlPipe
  ],
  imports: [
    CommonModule,
    VodListRoutingModule,
    HttpClientModule,
    NgxBootstrapIconsModule.pick({
      twitch
    }),
    NgbModule
  ],
  providers: [
    VodsService
  ]
})
export class VodListModule { }
