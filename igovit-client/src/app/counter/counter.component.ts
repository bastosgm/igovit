import { Component } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-counter-component',
  templateUrl: './counter.component.html',
  standalone: true,
  imports: [ReactiveFormsModule],
})
export class CounterComponent {
  public username = 'youngTech';
  public favoriteFramework = '';

  public currentCount = 0;

  public incrementCounter() {
    this.currentCount++;
  }
}
