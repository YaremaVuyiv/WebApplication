import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/Services/authentication.service';
import { ConfirmEmail } from 'src/Models/confirm-email.model';

@Component({
    templateUrl: '../Htmls/confirmEmail.component.html',
    styleUrls: ['../CSS/confirmEmail.component.css']
})
export class ConfirmEmailComponent implements OnInit {

    constructor(private router: Router, private accountService: AuthenticationService, private route: ActivatedRoute) {

     }

    ngOnInit() {
        let userId = '';
        let token = '';
        this.route.queryParams.subscribe(params => {
            userId = params['userId'];
            token = params['token'];
        });
        const confirmEmail = new ConfirmEmail();
        confirmEmail.token = token.split(' ').join('+');
        confirmEmail.userId = userId;
        this.accountService.confirmEmail(confirmEmail);
    }

    continue() {
        this.router.navigate(['/login']);
    }
}
