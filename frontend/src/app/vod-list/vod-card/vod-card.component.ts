import { AfterViewInit, Component, HostListener, Input, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';

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
  @Input()
  public date: Date;
  @Input()
  public viewCount: number;
  @Input()
  public vodId: number;

  public isImageLoaded: boolean;

  constructor(private router: Router) {
    this.thumbnailUrl = "";
    this.length = "";
    this.title = "";
    this.isImageLoaded = false;
    this.date = new Date();
    this.viewCount = 0;
    this.vodId = 0;
  }

  ngAfterViewInit(): void {
    this.image.nativeElement.onload = () => {
      this.isImageLoaded = true;
    }
  }

  handleClick() {
    this.router.navigate([`vods/${this.vodId}`])
  }
}
