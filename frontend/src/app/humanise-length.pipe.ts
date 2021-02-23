import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'humaniseLength' })
export class HumaniseLengthPipe implements PipeTransform {
  /**
   * Takes a value in seconds and converts it to the format hhh:mm:ss
   * @param value value to convert to the aforementioned format in seconds
   */
  transform(value: number) {
    const hours = ~~(value / 3600);
    const minutes = ~~((value % 3600) / 60);
    const seconds = ~~value % 60;
    let time = "";
    if (hours > 0) {
      time += hours + ":" + (minutes < 10 ? "0" : "");
    }
    time += minutes + ":" + (seconds < 10 ? "0" : "");
    time += seconds

    return time;
  }

}