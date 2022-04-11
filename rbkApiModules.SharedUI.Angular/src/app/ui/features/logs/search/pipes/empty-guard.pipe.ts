import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'emptyGuard'
})

export class EmptyGuardPipe implements PipeTransform {
  public transform(value: string, ...args: any[]): string {
    if (value != null && value !== '') {
      return value;
    }
    else {
      return args[0] ?? '-';
    }
  }
}