import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-vod-player',
  templateUrl: './vod-player.component.html',
  styleUrls: ['./vod-player.component.scss']
})
export class VodPlayerComponent implements OnInit {
  @Input("vodId")
  public vodId: string;

  public vodEmbedUrl: string;

  constructor() { 
    this.vodId = "";
    this.vodEmbedUrl = "";
  }

  ngOnInit(): void {
    this.vodEmbedUrl = `https://player.twitch.tv/?video=${this.vodId}&parent=localhost`;
  }
}
