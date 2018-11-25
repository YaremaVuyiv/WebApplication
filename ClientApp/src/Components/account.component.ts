import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpResponse } from '@angular/common/http'
import { User } from "../Models/User";
import { AuthenticationService } from "../Services/authentication.service";
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
    templateUrl: '../Htmls/account.component.html',
    providers: [AuthenticationService],
    styleUrls: ['../CSS/account.component.css']
})
export class AccountComponent implements OnInit {
    user = new User();
    isDataLoaded = false;

    constructor(private router: Router, private authService: AuthenticationService, private spinner: NgxSpinnerService) { }

    ngOnInit() {
        this.loadUserData();
    }

    loadUserData() {
        this.spinner.show();
        this.authService.getUserDetails().then((response: User) => {
            this.user = response;
            this.spinner.hide();
        });
    }

    changePassword() {
        this.router.navigate(['/changePassword']);
    }
}
