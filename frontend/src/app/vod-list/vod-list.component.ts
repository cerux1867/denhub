import { Component, OnInit } from '@angular/core';
import { VodsService } from '../vods.service';

@Component({
  selector: 'app-vod-list',
  templateUrl: './vod-list.component.html',
  styleUrls: ['./vod-list.component.scss']
})
export class VodListComponent implements OnInit {
  public vodList: Array<any>;
  private pageNum: number = 1;

  constructor(private vodsService: VodsService) {
    this.vodList = []
  }

  ngOnInit(): void {
    this.vodsService.getVods(this.pageNum, 20).subscribe((data) => {
      this.vodList = this.vodList.concat(data);
    });
  }

  onScrollDown() {
    this.pageNum++;
    this.vodsService.getVods(this.pageNum, 20).subscribe((data) => {
      this.vodList = this.vodList.concat(data);
    });
  }

}
