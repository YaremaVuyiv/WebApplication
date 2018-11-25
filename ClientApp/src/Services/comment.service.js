var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import 'rxjs/add/operator/toPromise';
var CommentService = /** @class */ (function () {
    function CommentService(http) {
        this.http = http;
        this.url = "/api/comment";
        var authToken = localStorage.getItem('token');
        this.headers = new Headers({
            'Content-Type': 'application/json',
            'Accept': 'q=0.8;application/json;q=0.9',
            'Authorization': "Bearer " + authToken,
        });
        this.options = new RequestOptions({ headers: this.headers });
    }
    CommentService.prototype.getAllComments = function (topicId) {
        return this.http.get(this.url + '/' + topicId, this.options).toPromise().then(function (response) {
            return response.json();
        });
    };
    CommentService.prototype.createComment = function (comment) {
        return this.http.post(this.url, JSON.stringify(comment), this.options).toPromise().then(function (response) { return response; });
    };
    CommentService.prototype.deleteComponent = function (id) {
        return this.http.delete(this.url + '/' + id, this.options).toPromise().then(function (response) { return response; });
    };
    CommentService.prototype.handleError = function (error) {
        console.error('An error occurred', error);
        return Promise.reject(error.message || error);
    };
    CommentService = __decorate([
        Injectable(),
        __metadata("design:paramtypes", [Http])
    ], CommentService);
    return CommentService;
}());
export { CommentService };
//# sourceMappingURL=comment.service.js.map