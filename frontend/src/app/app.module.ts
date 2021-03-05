import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { VodListComponent } from './vod-list/vod-list.component';
import { HumaniseLengthPipe } from './humanise-length.pipe';
import { TransformTwitchThumbnailUrlPipe } from './transform-twitch-thumbnail-url.pipe';
import { NgxBootstrapIconsModule, twitch } from 'ngx-bootstrap-icons';
import { LazyImgDirective } from './lazy-img.directive';
import { VodCardComponent } from './vod-card/vod-card.component';
import { HumaniseDateAgo } from './humanise-date-ago.pipe';
import { HumaniseViewsPipe } from './humanise-views.pipe';
@NgModule({
  declarations: [
    AppComponent,
    VodListComponent,
    HumaniseLengthPipe,
    HumaniseDateAgo,
    HumaniseViewsPipe,
    TransformTwitchThumbnailUrlPipe,
    LazyImgDirective,
    VodCardComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    NgbModule,
    HttpClientModule,
    NgxBootstrapIconsModule.pick({
      twitch
    })
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
