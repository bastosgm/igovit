import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup | any;
  status: string = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required]],
    });
  }

  onSubmit() {
    console.log(this.loginForm);
    if (!this.loginForm.valid) return;

    this.authService.login(this.loginForm.value).subscribe(
      (response: any) => {
        this.status = response.reasonPhrase;

        if (this.status != 'OK') {
          console.log('Erro no login', response);
          return;
        }

        this.router.navigate(['']);
        console.log('Login bem-sucedido', response);
      },
      (error) => {
        console.error('Erro no login', error);
      }
    );
  }
}
