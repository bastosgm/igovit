import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html',
})
export class FetchDataComponent {
  public users: User[] = [];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<User[]>(baseUrl + 'api/user').subscribe({
      next: (result) => (this.users = result),
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
