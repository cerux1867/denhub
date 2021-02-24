import { Component, OnInit } from '@angular/core';
import { VodsService } from '../vods.service';

@Component({
  selector: 'app-vod-list',
  templateUrl: './vod-list.component.html',
  styleUrls: ['./vod-list.component.scss']
})
export class VodListComponent implements OnInit {
  public vodList: Array<any>;
  public isLoading: boolean = false;
  public isFirstLoad: boolean = true;
  public loadedAll: boolean = false;

  private pageNum: number = 1;

  constructor(private vodsService: VodsService) {
    this.vodList = []
  }

  ngOnInit(): void {
    this.fetchVods();
    this.onScrollDown();
  }

  fetchVods(): void {
    this.isLoading = true;
    this.vodsService.getVods(this.pageNum, 15).subscribe((data) => {
      if (data.length) {
        this.vodList.push(...data);
      } else {
        this.loadedAll = true;
      }
      this.isLoading = false;
      this.isFirstLoad = false;
    });
  }

  onScrollDown(): void {
    window.onscroll = () => {
      const isBottom = this.isBottom();
      if (!isBottom && !this.loadedAll) {
        this.pageNum++;
        this.fetchVods();
      }
    }
  }

  isBottom(): boolean {
    if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight) {
      return false;
    }
    return true;
  }
}
