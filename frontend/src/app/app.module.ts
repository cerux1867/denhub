import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { VodListComponent } from './vod-list/vod-list.component';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { HumaniseLengthPipe } from './humanise-length.pipe';
import { TransformTwitchThumbnailUrlPipe } from './transform-twitch-thumbnail-url.pipe';
import { NgxBootstrapIconsModule, twitch } from 'ngx-bootstrap-icons';
@NgModule({
  declarations: [
    AppComponent,
    VodListComponent,
    HumaniseLengthPipe,
    TransformTwitchThumbnailUrlPipe
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    NgbModule,
    HttpClientModule,
    InfiniteScrollModule,
    NgxBootstrapIconsModule.pick({
      twitch
    })
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
