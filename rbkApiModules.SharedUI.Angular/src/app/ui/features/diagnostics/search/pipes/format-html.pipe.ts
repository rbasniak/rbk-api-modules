import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'formatHtml'
})

export class FormatHtmlPipe implements PipeTransform {
  public transform(value: string, ...args: any[]): string {
    if (value == null) return null;

    const parts = value?.split(/(?:\r\n|\r|\n)/g);

    let result = parts[0];
    if (result.length > args[0]) {
      result = result.substring(0, args[0]) + '...';
    }

    if (parts.length > 1) {
      result += '[click to expand]';
    }

    return result;
  }
}