import { Component, Inject, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../services/auth.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html',
})
export class FetchDataComponent implements OnInit, OnDestroy {
  public users: User[] = [];
  public isAuthenticated: boolean = false;

  private ngUnsubscribe = new Subject();

  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.fetchUsers();
    this.checkAuthentication();
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next(null);
    this.ngUnsubscribe.complete();
  }

  fetchUsers(): void {
    this.http
      .get<User[]>(this.baseUrl + 'api/user')
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (result) => (this.users = result),
        error: (error) => console.error(error),
      });
  }

  checkAuthentication(): void {
    this.authService
      .isAuthenticated()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (result) => (this.isAuthenticated = result),
        error: (error) => console.error(error),
      });
  }
}

interface User {
  // id: string;
  login: string;
  // password: string;
  status: string;
}
