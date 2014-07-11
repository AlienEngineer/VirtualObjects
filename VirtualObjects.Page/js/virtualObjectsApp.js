
        var app = angular.module('virtualObjectsApp', ['ngRoute']);

app.config(['$routeProvider',
    function($routeProvider) {
        $routeProvider.
                when('/ReleaseNotes', {
                    templateUrl: 'ReleaseNotes',
                    controller: 'ReleaseNotesController'
                }).
                otherwise({
                    templateUrl: 'Home',
                    controller: 'MainController'
                });
    }]);

/*
 
 
 
 
 */
var menu = {
    Home: {Class: '', Label: 'Home', Icon: 'fa-home'},
    GettingStartedScaffolding: {Class: '', Label: 'Scaffolding', Icon: 'fa-cogs'},
    GettingStarted: {Class: '', Label: 'Getting Started', Icon: 'fa-lightbulb-o'},
    CustomConfiguration: {Class: '', Label: 'Custom Configuration', Icon: 'fa-sliders'},
    POCOClasses: {Class: '', Label: 'POCO Classes', Icon: 'fa-male'},
    CrudOperations: {Class: '', Label: 'Crud Operations', Icon: 'fa-tasks'},
    LinqSupport: {Class: '', Label: 'Linq Support', Icon: 'fa-link'},
    EntityMapping: {Class: '', Label: 'Entity Mapping', Icon: 'fa-male'},
    ReleaseNotes: {Class: '', Label: 'ReleaseNotes', Icon: 'fa-history'},
    toArray: function() {
        var result = $.map(this, function(value, index) {
            return value;
        });

        result.length = result.length - 2;

        return result;
    }
};

menu.Active = menu.Home;

var switchPage = function($scope, item) {
    $scope.menu.Active.Class = '';
    item.Class = 'active';
    $scope.menu.Active = item;
};

app.controller('MenuController', function($scope) {
    $scope.menu = menu;
    $scope.items = menu.toArray();
    $scope.menu.Home.Class = 'active';
});


app.controller('ReleaseNotesController', function($scope) {
    $scope.menu = menu;
    switchPage($scope, $scope.menu.ReleaseNotes);
});

app.controller('MainController', function($scope) {
    $scope.menu = menu;
    switchPage($scope, $scope.menu.Home);
});

$(function() {
    Sunlight.highlightAll();

    $('#leftmenu').affix({
        offset: {
            top: 200
            ,
            bottom: function() {
                return (this.bottom = $('.footer').outerHeight(true))
            }
        }
    })
});
