import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-route1',
  template: '<h1>Rota 1</h1><p>Exemplo de rota com padding zerado.</p>',
})
export class Route1Component implements OnInit {
  public ngOnInit(): void {
  }
}