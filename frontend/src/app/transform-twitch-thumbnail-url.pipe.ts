import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'transformTwitchThumbnailUrl' })
export class TransformTwitchThumbnailUrlPipe implements PipeTransform {
  /**
   * Takes a URL with the following strings '%{height}%' and '%{width}%' and
   * replaces them with numerical values
   * @param value URL with strings '%{height}%' and '%{width}%'
   */
  transform(thumbnailUrl: string, height = 508, width = 320) {
    var widthTransformed = thumbnailUrl.replace('%{width}', `${height}`);
    var heightTransformed = widthTransformed.replace('%{height}', `${width}`);
    return heightTransformed;
  }

}