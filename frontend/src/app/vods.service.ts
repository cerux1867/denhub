import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class VodsService {

  constructor(private httpClient: HttpClient) { }

  public getVods(channelName: string, page = 1, limit = 10, title = ""): Observable<any[]> {
    let httpParams = new HttpParams()
      .set('page', page.toString())
      .set('limit', limit.toString());
    if (title) {
      httpParams = httpParams.set('title', title);
    }
    return this.httpClient.get<any[]>(`${environment.denhubApiUrl}/vods/${channelName}`, {
      params: httpParams
    }).pipe(map((vods: any[]) => vods.map((vod: any) => {
      return {
        title: vod.title,
        thumbnailUrl: vod.thumbnailUrl,
        length: vod.length,
        date: new Date(Date.parse(vod.date)),
        viewCount: vod.viewCount
      }
    })));
  }
}
