import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-access',
  templateUrl: './access.component.html',
  styleUrls: ['./access.component.css'],
})
export class AccessComponent implements OnInit {
  accessForm: FormGroup | any;
  message: string = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.accessForm = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required]],
    });
  }

  onSubmit() {
    console.log(this.accessForm);
    if (!this.accessForm.valid) return;

    this.authService.login(this.accessForm.value).subscribe(
      (response: any) => {
        if (response.reasonPhrase !== 'OK') {
          this.message = 'Login or password incorrect';
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
