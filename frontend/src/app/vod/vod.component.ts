import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-vod',
  templateUrl: './vod.component.html',
  styleUrls: ['./vod.component.scss']
})
export class VodComponent implements OnInit {
  public vodId: string;

  constructor(private route: ActivatedRoute) { 
    this.vodId = "";
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.vodId = params['id'];
    });
  }
}
