import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'transformTwitchThumbnailUrl' })
export class TransformTwitchThumbnailUrlPipe implements PipeTransform {
  /**
   * Turns a twitch thumbnail URL with placeholder parameters into a width and height enriched thumbnail URL
   * @param thumbnailUrl Thumbnail URL with placeholder params for width and height
   * @param height Height in pixels of the desired thumbnail image. Defaults to 720.
   * @param width Width in pixels of the desired thumbnail image. Defaults to 1280.
   */
  transform(thumbnailUrl: string, height = 720, width = 1280) {
    var widthTransformed = thumbnailUrl.replace('%{width}', `${width}`);
    var heightTransformed = widthTransformed.replace('%{height}', `${height}`);
    return heightTransformed;
  }
}