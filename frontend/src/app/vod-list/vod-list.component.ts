import { Component, OnInit } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { VodsService } from '../vods.service';
import { IChannel } from './IChannel';

@Component({
  selector: 'app-vod-list',
  templateUrl: './vod-list.component.html',
  styleUrls: ['./vod-list.component.scss']
})
export class VodListComponent implements OnInit {
  public isLoading: boolean = false;
  public isFirstLoad: boolean = true;
  public loadedAll: boolean = false;
  public subject = new Subject<any>();
  public availableChannelList = new Array<IChannel>({
    displayName: 'EsfandTV',
    channelName: 'esfandtv'
  });
  public selectedChannel: IChannel = this.availableChannelList[0];

  public vodList$: Observable<any>;
  public subject$ = this.subject.asObservable();
  public vodList: Array<any>;

  private _pageNum: number = 1;
  private _titleFilter: string = "";

  constructor(private vodsService: VodsService) {
    this.vodList = []
    this.vodList$ = this.subject$.pipe(
      switchMap(() => {
        return this.vodsService.getVods(this.selectedChannel.channelName, this._pageNum, 15, this._titleFilter)
      })
    );
    this.vodList$.subscribe((data) => {
      if (data.length) {
        this.vodList.push(...data);
      } else {
        this.loadedAll = true;
      }
      this.isLoading = false;
      this.isFirstLoad = false;
    });
  }

  ngOnInit(): void {
    this.fetchVods();
    this.onScrollDown();
  }

  onChannelChange(channel: any): void {
    this.selectedChannel = channel;
    this.resetVodList();
    this.fetchVods();
  }

  onTitleFilterInputChange(eventTarget: any): void {
    if (eventTarget.value.length >= 3) {
      this._titleFilter = eventTarget.value.trim();
      this.resetVodList();
      this.fetchVods();
    } else if (eventTarget.value === "") {
      this._titleFilter = "";
      this.resetVodList();
      this.fetchVods();
    }
  }

  fetchVods(): void {
    this.isLoading = true;
    this.subject.next();
  }

  onScrollDown(): void {
    window.onscroll = () => {
      const isBottom = this.isBottom();
      if (!isBottom && !this.loadedAll && !this.isLoading) {
        this._pageNum++;
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

  resetVodList(): void {
    this.vodList = [];
    this.isFirstLoad = true;
    this.loadedAll = false;
    this._pageNum = 1;
  }
}
