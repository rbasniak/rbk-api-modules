import { Pipe, PipeTransform } from '@angular/core';
import { LogLevel } from '@models/logs/logs-entry';

@Pipe({
  name: 'formatLevel'
})

export class FormatLevelPipe implements PipeTransform {
  public transform(value: LogLevel): string {
    if (value === LogLevel.Warning) {
      return 'Warning';
    }
    else if (value === LogLevel.Error) {
      return 'Error';
    }
    else if (value === LogLevel.Debug) {
      return 'Debug';
    }
    else {
      return 'Without level';
    }
  }
}