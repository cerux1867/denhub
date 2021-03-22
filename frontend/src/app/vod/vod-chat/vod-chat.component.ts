import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-vod-chat',
  templateUrl: './vod-chat.component.html',
  styleUrls: ['./vod-chat.component.scss']
})
export class VodChatComponent implements OnInit {
  @Input('isLoading')
  public isLoading: boolean;

  constructor() { 
    this.isLoading = false;
  }

  ngOnInit(): void {
  }

}
