var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from "../Services/authentication.service";
var ChangePasswordComponent = /** @class */ (function () {
    function ChangePasswordComponent(router, authService) {
        this.router = router;
        this.authService = authService;
        this.isDataLoaded = false;
    }
    ChangePasswordComponent.prototype.ngOnInit = function () {
    };
    ChangePasswordComponent.prototype.changePassword = function () {
        this.authService.changePassword(this.oldPasswordText, this.newPasswordText).then();
    };
    ChangePasswordComponent = __decorate([
        Component({
            templateUrl: '../Htmls/changePassword.component.html',
            providers: [AuthenticationService],
            styleUrls: ['../CSS/changePassword.component.css']
        }),
        __metadata("design:paramtypes", [Router, AuthenticationService])
    ], ChangePasswordComponent);
    return ChangePasswordComponent;
}());
export { ChangePasswordComponent };
//# sourceMappingURL=changePassword.component.js.map