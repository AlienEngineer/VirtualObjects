<?php include 'views/topmenu.html' ?>    

<div class="jumbotron hidden-xs">
    <img class="hidden-xs col-md-3 col-sm-4 col-lg-2 col-xs-3" src="images/1391724649_Database_3.png"  />
    <h1>VirtualObjects</h1>
    <p>Easy and Fast ORM</p>   
</div>

<div class="row">
    <div class="col-md-3" ng-controller="MenuController">
        <?php include 'leftmenu.html' ?>
    </div>
    <div class="col-md-9">
        <div ng-view></div>
    </div>
</div>
