import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';

@Component({
  selector: 'app-vod-card',
  templateUrl: './vod-card.component.html',
  styleUrls: ['./vod-card.component.scss']
})
export class VodCardComponent implements AfterViewInit {
  @ViewChild('image')
  public image: any;
  
  @Input()
  public thumbnailUrl: string;
  @Input()
  public length: string;
  @Input()
  public title: string;

  public isImageLoaded: boolean;

  constructor() { 
    this.thumbnailUrl = "";
    this.length = "";
    this.title = "";
    this.isImageLoaded = false;
  }

  ngAfterViewInit(): void {
    this.image.nativeElement.onload =() => {
      this.isImageLoaded = true;
    }
  }
}
