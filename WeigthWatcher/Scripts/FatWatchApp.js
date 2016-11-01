

var FatWatchApp = angular.module('FatWatchApp', ['ngRoute']);

FatWatchApp.controller('LandingPageController', LandingPageController);

var configFunction = function ($routeProvider) {
    $routeProvider.
        when('/doctors', {
            templateUrl: 'doctors/index'
        })
        .when('/doctors/add', {
            templateUrl: 'doctors/add'
        });
    //.when('/routeThree', {
    //    templateUrl: 'doctors/three'
    //});
}

configFunction.$inject = ['$routeProvider'];

FatWatchApp.config(configFunction);

