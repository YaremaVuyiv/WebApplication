﻿import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from "../Services/authentication.service";
import { FormsModule, ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { User } from "../Models/User";
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
    templateUrl: '../Htmls/register.component.html',
    providers: [AuthenticationService],
    styleUrls: ['../CSS/register.component.css']
})

export class RegisterComponent {
    user: User = new User("", "", "", "", "");
    emailPattern = "^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$";

    constructor(
        private router: Router,
        private authenticationService: AuthenticationService, private spinner: NgxSpinnerService) { }

    register() {
        this.spinner.show();
        this.authenticationService.register(this.user).then(
            data => {
                //parent.document.getElementById("emailConfirmAlert").hidden = false;
                this.router.navigate(['/']);
                this.spinner.hide();
                        })
            .catch(error => console.log(error));
    }
}